using System;
using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Describes a sequence option: an option that can take any number
    /// of arguments. Each argument is parsed identically and the result
    /// of the option is formatted as an IReadOnlyList&lt;T&gt;.
    /// </summary>
    public sealed class SequenceOption<T> : Option
    {
        /// <summary>
        /// Creates a sequence option from an option form
        /// and a function that parses a single argument.
        /// </summary>
        /// <param name="form">
        /// The option's form.
        /// </param>
        /// <param name="parseArgument">
        /// A function that parses a single string argument.
        /// </param>
        public SequenceOption(
            OptionForm form,
            Func<string, ILog, T> parseArgument)
            : this(new OptionForm[] { form }, parseArgument)
        { }

        /// <summary>
        /// Creates a sequence option from a list of forms
        /// and a function that parses a single argument.
        /// </summary>
        /// <param name="forms">
        /// The list of forms that the sequence option accepts.
        /// </param>
        /// <param name="parseArgument">
        /// A function that parses a single string argument.
        /// </param>
        public SequenceOption(
            IReadOnlyList<OptionForm> forms,
            Func<string, ILog, T> parseArgument)
        {
            this.forms = forms;
            this.parseArgument = parseArgument;
        }

        private IReadOnlyList<OptionForm> forms;
        private Func<string, ILog, T> parseArgument;

        /// <inheritdoc/>
        public override IReadOnlyList<OptionForm> Forms => forms;

        /// <inheritdoc/>
        public override object DefaultValue => new T[] { };

        /// <inheritdoc/>
        public override OptionParser CreateParser(OptionForm form)
        {
            return new SequenceOptionParser<T>(parseArgument);
        }

        /// <inheritdoc/>
        public override ParsedOption MergeValues(ParsedOption first, ParsedOption second)
        {
            // Listen to the last flag provided by the user.
            var result = new List<T>((IReadOnlyList<T>)first.Value);
            result.AddRange((IReadOnlyList<T>)second.Value);
            return new ParsedOption(first.Form, result);
        }
    }

    /// <summary>
    /// Helps build sequence options.
    /// </summary>
    public static class SequenceOption
    {
        /// <summary>
        /// Creates a string-sequence option that takes a single form.
        /// </summary>
        /// <param name="form">The string-sequence option's form.</param>
        /// <returns>A string-sequence option.</returns>
        public static SequenceOption<string> CreateStringOption(OptionForm form)
        {
            return new SequenceOption<string>(form, parseStringArgument);
        }

        /// <summary>
        /// Creates a string-sequence option that takes a list of forms.
        /// </summary>
        /// <param name="forms">The string-sequence option's forms.</param>
        /// <returns>A string-sequence option.</returns>
        public static SequenceOption<string> CreateStringOption(IReadOnlyList<OptionForm> forms)
        {
            return new SequenceOption<string>(forms, parseStringArgument);
        }

        /// <summary>
        /// Creates a string-sequence option that takes a list of forms.
        /// </summary>
        /// <param name="forms">The string-sequence option's forms.</param>
        /// <returns>A string-sequence option.</returns>
        public static SequenceOption<string> CreateStringOption(params OptionForm[] forms)
        {
            return new SequenceOption<string>(forms, parseStringArgument);
        }

        private static string parseStringArgument(string argument, ILog log)
        {
            return argument;
        }
    }

    internal sealed class SequenceOptionParser<T> : OptionParser
    {
        public SequenceOptionParser(Func<string, ILog, T> parseArgument)
        {
            this.parseArgument = parseArgument;
            this.arguments = new List<string>();
        }

        private Func<string, ILog, T> parseArgument;
        private List<string> arguments;

        public override object GetValue(ILog log)
        {
            var results = new T[arguments.Count];
            for (int i = 0; i < arguments.Count; i++)
            {
                results[i] = parseArgument(arguments[i], log);
            }
            return results;
        }

        public override bool Parse(string argument)
        {
            // Take as many arguments as you can.
            arguments.Add(argument);
            return true;
        }
    }
}