using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pixie;
using Pixie.Markup;
using Pixie.Options;
using Pixie.Terminal;
using Pixie.Transforms;

namespace ParseOptions
{
    public static class Program
    {
        // A number of option definitions.

        // This option is a simple flag. It takes no arguments.
        private static readonly FlagOption optimizeFastFlag =
            Option.Flag("-Ofast")
            .WithCategory("Optimization options")
            .WithDescription("Enable aggressive optimizations.");

        // This option is also a simple flag, but it has two
        // different forms:
        //     -h and --help.
        private static readonly FlagOption helpFlag =
            Option.Flag("-h", "--help")
            .WithDescription(
                "Print a description of the options understood.");

        // This option has both a positive and a negative form:
        //     -fsyntax-only and -fno-syntax-only.
        private static readonly FlagOption syntaxOnlyFlag =
            Option.Toggle(
                "-fsyntax-only",
                "-fno-syntax-only")
            .WithDescription("Check the code for syntax errors only.");

        // This option takes zero or more strings as arguments.
        private static readonly SequenceOption<string> filesOption =
            Option.StringSequence("--files")
            .WithDescription("Consume files as input.")
            .WithParameter("file");

        // This option takes a 32-bit signed integer as argument.
        private static readonly ValueOption<int> optimizeOption =
            Option.Int32WithDefault(
                0,
                "-O")
            .WithCategory("Optimization options")
            .WithDescription("Pick an optimization level.")
            .WithParameter("n");

        // This option takes a 32-bit signed integer as argument.
        // It also happens to have two forms.
        private static readonly ValueOption<int> maxErrorsOption =
            Option.Int32WithDefault(
                0,
                "-fmax-errors",
                "-ferror-limit")
            .WithDescription(
                new Sequence(
                    new MarkupNode[]
                    {
                        "Limits the maximum number of error messages to ",
                        new SymbolicOptionParameter("n").Representation,
                        "."
                    }))
            .WithParameter("n");
        public static void Main(string[] args)
        {
            // First, acquire a terminal log. You should acquire
            // a log once and then re-use it in your application.
            ILog log = TerminalLog
                .Acquire()
                .WithDiagnostics("program")
                .WithWordWrap();

            var allOptions = new Option[]
            {
                filesOption,
                syntaxOnlyFlag,
                optimizeOption,
                optimizeFastFlag,
                helpFlag,
                maxErrorsOption
            };

            // `filesOption` plays two roles in this example:
            // it can be spelled explicitly as `--files`,
            // and it is also the option that should consume bare
            // positional arguments such as `a.txt`.
            //
            // The third constructor argument picks the form Pixie should
            // mention when it needs to describe that positional behavior
            // in help or diagnostics.
            var commandLine = new CommandLine(
                allOptions, filesOption, filesOption.Forms[0]);

            var parsedOptions = commandLine.Parse(args, log);

            if (!parsedOptions.IsSuccess || parsedOptions.WasHandled)
            {
                return;
            }

            if (!parsedOptions.GetValue<bool>(syntaxOnlyFlag))
            {
                log.Info(RenderParsedOptions(allOptions, parsedOptions));
            }
        }

        private static MarkupNode RenderParsedOptions(
            IReadOnlyList<Option> options,
            OptionParseResult parsedOptions)
        {
            return new BulletedList(
                options
                    .Select(opt => RenderParsedOption(opt, parsedOptions))
                    .ToArray(),
                true);
        }

        private static MarkupNode RenderParsedOption(
            Option opt,
            OptionParseResult parsedOptions)
        {
            // Pixie can accept many forms for the same option, such as
            // `-h` and `--help`.
            //
            // For this little report, we print the option's first form as
            // its canonical display name. That keeps the output stable and
            // makes it obvious which spelling the example treats as the
            // primary one, even though the parser still accepts the others.
            return new Sequence(
                DecorationSpan.MakeBold(opt.Forms[0].ToString()),
                ": ",
                ValueToMarkup(parsedOptions.GetValue<object>(opt)));
        }

        private static MarkupNode ValueToMarkup(object value)
        {
            // Sequence options come back as enumerable values, so format them
            // as `{ a, b, c }` to make it clear that multiple arguments were
            // gathered into one parsed option value.
            if (value is IEnumerable sequence && !(value is string))
            {
                var sb = new StringBuilder();
                sb.Append("{ ");
                bool first = true;
                foreach (var element in sequence)
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ValueToText(element));
                    first = false;
                }
                sb.Append(" }");
                return new Text(sb.ToString());
            }

            return new Text(ValueToText(value));
        }

        private static string ValueToText(object value)
        {
            return value == null ? "" : value.ToString();
        }
    }
}
