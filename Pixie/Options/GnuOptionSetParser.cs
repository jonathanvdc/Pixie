using System;
using System.Collections.Generic;
using System.Linq;
using Pixie.Markup;

namespace Pixie.Options
{
    /// <summary>
    /// A parser for GNU-style option sets.
    /// </summary>
    public sealed class GnuOptionSetParser : OptionSetParser
    {
        /// <summary>
        /// Creates a GNU-style option parser from a list of
        /// options, a default option and a preferred form
        /// for that default option.
        /// </summary>
        /// <param name="options">
        /// A list of all options known to this parser.
        /// </param>
        /// <param name="defaultOption">
        /// The default option: the option that
        /// is implicitly parsed when no other option
        /// can accept arguments.
        /// </param>
        /// <param name="defaultForm">
        /// The preferred form for the default option.
        /// </param>
        public GnuOptionSetParser(
            IReadOnlyList<Option> options,
            Option defaultOption,
            OptionForm defaultForm)
        {
            this.Options = options;
            this.DefaultOption = defaultOption;
            this.DefaultForm = defaultForm;
            this.shortForms = new TrieNode<char, Option>();
            this.longForms = new Dictionary<string, Option>();
            PopulateDataStructures();
        }

        /// <summary>
        /// Gets a list of all options known to this parser.
        /// </summary>
        /// <returns>A list of options.</returns>
        public IReadOnlyList<Option> Options { get; private set; }

        /// <summary>
        /// Gets the default option: the option that
        /// is implicitly parsed when no other option
        /// can accept arguments.
        /// </summary>
        /// <returns>The default option.</returns>
        public Option DefaultOption { get; private set; }

        /// <summary>
        /// Gets the preferred form for the default option.
        /// </summary>
        /// <returns>The default option's default form.</returns>
        public OptionForm DefaultForm { get; private set; }

        // A trie of short option forms.
        internal TrieNode<char, Option> shortForms;

        // A mapping of long form strings to options.
        internal Dictionary<string, Option> longForms;

        /// <inheritdoc/>
        public override OptionSet Parse(
            IReadOnlyList<string> arguments,
            ILog log)
        {
            var state = new GnuOptionSetParserState(this, log);
            state.StartParsing(DefaultOption, DefaultForm);

            // Drip-feed the arguments to the state.
            int argCount = arguments.Count;
            for (int i = 0; i < argCount; i++)
            {
                // Parse an argument.
                if (!state.Parse(arguments[i]))
                {
                    log.Log(
                        new LogEntry(
                            Severity.Error,
                            "meaningless argument",
                            new Text("cannot assign a meaning to excessive command-line argument "),
                            Quotation.CreateBoldQuotation(arguments[i]),
                            new Text(".")));
                    break;
                }
            }

            // Finish parsing all options.
            state.FinishParsingAll();

            return new OptionSet(state.ParsedOptions);
        }

        private void PopulateDataStructures()
        {
            var optCount = Options.Count;
            for (int i = 0; i < optCount; i++)
            {
                var opt = Options[i];
                var optForms = opt.Forms;
                int formCount = optForms.Count;
                for (int j = 0; j < formCount; j++)
                {
                    var form = optForms[j];
                    if (form.IsShort)
                    {
                        shortForms.AddPath(form.Name, opt);
                    }
                    else
                    {
                        longForms[form.Name] = opt;
                    }
                }
            }
        }
    }

    internal struct GnuOptionSetParserState
    {
        public GnuOptionSetParserState(GnuOptionSetParser parser, ILog log)
        {
            this = default(GnuOptionSetParserState);
            this.Parser = parser;
            this.Log = log;
            this.parsedOpts = new Dictionary<Option, ParsedOption>();
            this.parseStack = new Stack<GnuOptionParseState>();
        }

        /// <summary>
        /// Gets the parser that created this state.
        /// </summary>
        /// <returns>The parser.</returns>
        public GnuOptionSetParser Parser { get; private set; }

        /// <summary>
        /// Gets a log to which messages can be sent while parsing
        /// options.
        /// </summary>
        /// <returns>A log.</returns>
        public ILog Log { get; private set; }

        /// <summary>
        /// Gets the options that have been parsed by this state.
        /// </summary>
        /// <returns>The options that have been parsed by this state.</returns>
        public IReadOnlyDictionary<Option, ParsedOption> ParsedOptions => parsedOpts;

        // A dictionary of options that have been parsed.
        private Dictionary<Option, ParsedOption> parsedOpts;

        // A stack of options that are currently being parsed.
        private Stack<GnuOptionParseState> parseStack;

        /// <summary>
        /// Tells if this state is done parsing.
        /// </summary>
        public bool IsDone => parseStack.Count == 0;

        /// <summary>
        /// Parses a string argument.
        /// </summary>
        /// <param name="argument">An argument.</param>
        /// <returns><c>true</c> if the argument could be parsed; otherwise, <c>false</c>.</returns>
        public bool Parse(string argument)
        {
            string trimmedArg;
            switch (Classify(argument, out trimmedArg))
            {
                case GnuArgumentType.LongOption:
                    return ParseLongOption(trimmedArg);
                case GnuArgumentType.ShortOption:
                    return ParseShortOption(trimmedArg);
                default:
                    return ParseArgument(trimmedArg);
            }
        }

        private bool ParseLongOption(string argument)
        {
            string key, value;
            if (IsKeyValueOption(argument, out key, out value))
            {
                // Key-value options like '--help=warnings'
                // get special treatment.
                var opt = Parser.longForms[key];
                ParseKeyValueOption(opt, new OptionForm(key, false), value);
                return true;
            }
            else
            {
                // Non--key-value long-form options never take
                // any arguments directly. All we need to do is
                // figure out which option the user is referring to
                // and push it onto the option stack.
                Option opt;
                if (!Parser.longForms.TryGetValue(argument, out opt))
                {
                    // Log that the option is unrecognized and early-out.
                    LogUnrecognized("--" + argument);
                    return true;
                }

                // Start parsing the option.
                StartParsing(
                    opt,
                    new OptionForm(argument, false));

                return true;
            }
        }

        private bool ParseShortOption(string argument)
        {
            string key, value;
            if (IsKeyValueOption(argument, out key, out value))
            {
                // Key-value options like '-Wframe-larger-than=len'
                // get special treatment.
                var opt = Parser.shortForms.Get(key);
                ParseKeyValueOption(opt, new OptionForm(key, true), value);
                return true;
            }
            else
            {
                // This is the case of a typical short-form option, e.g.,
                //
                //     -o a.out
                //     -oa.out
                //     -Wnoexcept
                //     -Wnoexcept-type ( <-- this option and -Wnoexcept need to coexist)

                // Find the longest match.
                var node = Parser.shortForms;
                var opt = node.Value;
                int longestMatch = 0;
                int i = 0;
                foreach (var c in argument)
                {
                    if (!node.TryGetNext(c, out node))
                    {
                        break;
                    }

                    i++;
                    if (node.Value != null)
                    {
                        opt = node.Value;
                        longestMatch = i;
                    }
                }

                if (opt == null)
                {
                    LogUnrecognized("-" + argument);
                    return true;
                }

                // Start parsing the option.
                StartParsing(
                    opt,
                    new OptionForm(
                        argument.Substring(0, longestMatch),
                        true));

                if (longestMatch == argument.Length)
                {
                    // The entire option has been parsed; we're done here.
                    return true;
                }
                else
                {
                    // Okay, so this is where things get tricky. We've parsed
                    // an option followed by some junk and we don't know what
                    // it means.
                    //
                    // For example:
                    //
                    //     -O1
                    //       ^
                    //
                    //     -xzf source.tar.gz
                    //       ^
                    //
                    // We could be dealing with either an argument to the
                    // option we just parsed or with a new option.
                    //
                    // TODO: GCC doesn't accept the second example. Maybe we
                    // shouldn't be guessing here and instead let a flag
                    // control what to do when we reach this case.
                    value = argument.Substring(longestMatch);
                    var hasParsed = parseStack.Peek().Parser.Parse(value);
                    if (hasParsed)
                    {
                        // Successfully parsed as argument.
                        return true;
                    }
                    else
                    {
                        // Try to parse it as an option.
                        return ParseShortOption(value);
                    }
                }
            }
        }

        private void ParseKeyValueOption(Option opt, OptionForm form, string value)
        {
            if (opt == null)
            {
                LogUnrecognized(form.ToString());
            }
            else
            {
                StartParsing(opt, form);
                var hasParsed = parseStack.Peek().Parser.Parse(value);
                FinishParsing();
                if (!hasParsed)
                {
                    LogUnexpectedArgument(form.ToString(), value);
                }
            }
        }

        private void LogUnexpectedArgument(string key, string value)
        {
            Log.Log(
                new LogEntry(
                    Severity.Error,
                    "unexpected argument",
                    new Text("command line option did not expect an argument "),
                    Quotation.CreateBoldQuotation(key),
                    new Text(", but was given argument "),
                    Quotation.CreateBoldQuotation(value),
                    new Text(".")));
        }

        private void LogUnrecognized(string option)
        {
            Log.Log(
                new LogEntry(
                    Severity.Error,
                    "unknown option",
                    new Text("unrecognized command line option "),
                    Quotation.CreateBoldQuotation(option),
                    new Text(".")));
        }

        private bool ParseArgument(string argument)
        {
            if (IsDone)
            {
                return false;
            }

            bool accepted = parseStack.Peek().Parser.Parse(argument);
            if (accepted)
            {
                return true;
            }
            else
            {
                // Top-of-stack parser is done parsing. Pop it and move
                // to the next parser.
                FinishParsing();
                return ParseArgument(argument);
            }
        }

        private static GnuArgumentType Classify(
            string argument,
            out string trimmedArg)
        {
            if (argument.StartsWith("--") && argument.Length > 2)
            {
                trimmedArg = argument.Substring(2);
                return GnuArgumentType.LongOption;
            }
            else if (argument.StartsWith("-") && argument.Length > 1)
            {
                trimmedArg = argument.Substring(1);
                return GnuArgumentType.ShortOption;
            }
            else
            {
                trimmedArg = argument;
                return GnuArgumentType.Argument;
            }
        }

        /// <summary>
        /// Tells if an option is a key-value option, i.e.,
        /// it has format 'key=value'.
        /// </summary>
        /// <param name="option">The (trimmed) option.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if the option is a key-value option; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsKeyValueOption(
            string option,
            out string key,
            out string value)
        {
            int index = option.IndexOf('=');
            if (index <= 0)
            {
                key = null;
                value = null;
                return false;
            }
            else
            {
                key = option.Substring(0, index - 1);
                value = option.Substring(index + 1);
                return true;
            }
        }

        /// <summary>
        /// Starts parsing a particular form of an option.
        /// </summary>
        /// <param name="option">The option to parse.</param>
        /// <param name="form">The form of the option to parse.</param>
        public void StartParsing(Option option, OptionForm form)
        {
            parseStack.Push(
                new GnuOptionParseState(
                    option,
                    form,
                    option.CreateParser(form)));
        }

        /// <summary>
        /// Finishes parsing the top-of-stack option.
        /// </summary>
        private void FinishParsing()
        {
            FinishParsing(parseStack.Pop());
        }

        /// <summary>
        /// Finishes parsing an option.
        /// </summary>
        /// <param name="opt">The option to finish parsing.</param>
        private void FinishParsing(GnuOptionParseState opt)
        {
            var newVal = new ParsedOption(
                opt.Form,
                opt.Parser.GetValue(Log));

            ParsedOption oldVal;
            if (parsedOpts.TryGetValue(opt.Option, out oldVal))
            {
                newVal = opt.Option.MergeValues(oldVal, newVal);
            }

            parsedOpts[opt.Option] = newVal;
        }

        /// <summary>
        /// Finishes parsing all options.
        /// </summary>
        public void FinishParsingAll()
        {
            // When the last argument has been parsed, we need
            // to be careful with the order in which we finish
            // parsing options.
            //
            // For example, consider the following:
            //
            //     gcc -x c++ a.cpp -x c++ b.cpp
            //                                  ^
            //
            // When the parser is at the caret, its option stack
            // will look like this:
            //
            //     | -x c++ b.cpp | <-- top of stack
            //     | -x c++ a.cpp |
            //     | --source     |
            //
            // If were to finish parsing the top-of-stack option
            // first, then the arguments to '-x' will be merged
            // like so: 'c++ b.cpp' ++ 'c++ a.cpp'.
            //
            // But that's not what we want! If the user specifies
            // 'a.cpp' first, then the option parser shouldn't
            // surreptitiously re-arrange that after the last
            // argument has been parsed.
            //
            // To solve this issue, we finish parsing options in
            // the order in which we started parsing them, i.e.,
            // in reverse stack order.

            var stackArr = parseStack.ToArray();
            for (int i = stackArr.Length - 1; i >= 0; i--)
            {
                FinishParsing(stackArr[i]);
            }
        }
    }

    internal struct GnuOptionParseState
    {
        public GnuOptionParseState(
            Option option,
            OptionForm form,
            OptionParser parser)
        {
            this = default(GnuOptionParseState);
            this.Option = option;
            this.Form = form;
            this.Parser = parser;
        }

        /// <summary>
        /// Gets the option that is being parsed.
        /// </summary>
        /// <returns>The option that is being parsed.</returns>
        public Option Option { get; private set; }

        /// <summary>
        /// Gets the form of the option that is being parsed.
        /// </summary>
        /// <returns>The form of the option that is being parsed.</returns>
        public OptionForm Form { get; private set; }

        /// <summary>
        /// Gets the option parser for the option being parsed.
        /// </summary>
        /// <returns>The option parser.</returns>
        public OptionParser Parser { get; private set; }
    }

    /// <summary>
    /// An enumeration of ways command-line arguments can
    /// be classified.
    /// </summary>
    internal enum GnuArgumentType
    {
        Argument,

        ShortOption,

        LongOption
    }

    /// <summary>
    /// A simple trie implementation.
    /// </summary>
    internal sealed class TrieNode<TKey, TValue>
    {
        /// <summary>
        /// Creates a trie node.
        /// </summary>
        public TrieNode() : this(default(TValue))
        { }

        /// <summary>
        /// Creates a trie node from a value.
        /// </summary>
        /// <param name="value">The trie node's value.</param>
        public TrieNode(
            TValue value)
        {
            this.Value = value;
            this.successors = new Dictionary<TKey, TrieNode<TKey, TValue>>();
        }

        /// <summary>
        /// Gets the value associated with this trie node.
        /// </summary>
        /// <returns>The value for this node.</returns>
        public TValue Value { get; set; }

        private Dictionary<TKey, TrieNode<TKey, TValue>> successors;

        /// <summary>
        /// Gets or sets the next trie node obtained by taking the
        /// outgoing edge associated with a particular key.
        /// </summary>
        public TrieNode<TKey, TValue> this[TKey key]
        {
            get { return successors[key]; }
            set { successors[key] = value; }
        }

        /// <summary>
        /// Tries to get the next trie node by taking the outgoing
        /// edge associated with a given key.
        /// </summary>
        /// <param name="key">The key that determines which node to pick.</param>
        /// <param name="next">The next node.</param>
        /// <returns><c>true</c></returns>
        public bool TryGetNext(TKey key, out TrieNode<TKey, TValue> next)
        {
            return successors.TryGetValue(key, out next);
        }

        /// <summary>
        /// Adds a path to this trie that ends in a particular value.
        /// Existing values are overwritten.
        /// </summary>
        /// <param name="path">
        /// The keys that constitute the path through this trie.
        /// </param>
        /// <param name="value">
        /// The value at the end of the path.
        /// </param>
        public void AddPath(IEnumerable<TKey> path, TValue value)
        {
            var curNode = this;
            foreach (var key in path)
            {
                TrieNode<TKey, TValue> nextNode;
                if (!curNode.TryGetNext(key, out nextNode))
                {
                    nextNode = new TrieNode<TKey, TValue>(default(TValue));
                    curNode[key] = nextNode;
                }
                curNode = nextNode;
            }
            curNode.Value = value;
        }

        /// <summary>
        /// Gets the value obtained by traversing a specific path
        /// of keys. The default value is returned if the path
        /// is not in this trie.
        /// </summary>
        /// <param name="path">The path of keys to follow.</param>
        /// <returns>The value obtained by traversing the path.</returns>
        public TValue Get(IEnumerable<TKey> path)
        {
            var curNode = this;
            foreach (var key in path)
            {
                TrieNode<TKey, TValue> nextNode;
                if (curNode.TryGetNext(key, out nextNode))
                {
                    curNode = nextNode;
                }
                else
                {
                    curNode = null;
                    break;
                }
            }

            if (curNode == null)
            {
                return default(TValue);
            }
            else
            {
                return curNode.Value;
            }
        }
    }
}