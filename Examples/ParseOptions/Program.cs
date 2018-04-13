using System;
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
            new FlagOption(OptionForm.Short("Ofast"))
            .WithCategory("Optimization options")
            .WithDescription("Enable aggressive optimizations.");

        // This option is also a simple flag, but it has two
        // different forms:
        //     -h and --help.
        private static readonly FlagOption helpFlag =
            FlagOption.CreateFlagOption(
                new OptionForm[]
                {
                    OptionForm.Short("h"),
                    OptionForm.Long("help")
                })
            .WithDescription(
                "Print a description of the options understood.");

        // This option has both a positive and a negative form:
        //     -fsyntax-only and -fno-syntax-only.
        private static readonly FlagOption syntaxOnlyFlag =
            new FlagOption(
                OptionForm.Short("fsyntax-only"),
                OptionForm.Short("fno-syntax-only"),
                false)
            .WithDescription("Check the code for syntax errors only.");

        // This option takes zero or more strings as arguments.
        private static readonly SequenceOption<string> filesOption =
            SequenceOption.CreateStringOption(
                OptionForm.Long("files"))
            .WithDescription("Consume files as input.")
            .WithParameters(
                new OptionParameter[]
                {
                    new SymbolicOptionParameter("file", true)
                });

        // This option takes a 32-bit signed integer as argument.
        private static readonly ValueOption<int> optimizeOption =
            ValueOption.CreateInt32Option(
                OptionForm.Short("O"),
                0)
            .WithCategory("Optimization options")
            .WithDescription("Pick an optimization level.")
            .WithParameter(new SymbolicOptionParameter("n"));

        // This option takes a 32-bit signed integer as argument.
        // It also happens to have two forms.
        private static readonly ValueOption<int> maxErrorsOption =
            ValueOption.CreateInt32Option(
                new OptionForm[]
                {
                    OptionForm.Short("fmax-errors"),
                    OptionForm.Short("ferror-limit")
                },
                0)
            .WithDescription(
                new Sequence(
                    new MarkupNode[]
                    {
                        "Limits the maximum number of error messages to ",
                        new SymbolicOptionParameter("n").Representation,
                        "."
                    }))
            .WithParameter(new SymbolicOptionParameter("n"));

        private static OptionSet parsedOptions;

        public static void Main(string[] args)
        {
            // First, acquire a terminal log. You should acquire
            // a log once and then re-use it in your application.
            ILog log = TerminalLog.Acquire();

            log = new TransformLog(
                log,
                new Func<LogEntry, LogEntry>[]
                {
                    MakeDiagnostic
                });

            var allOptions = new Option[]
            {
                filesOption,
                syntaxOnlyFlag,
                optimizeOption,
                optimizeFastFlag,
                helpFlag,
                maxErrorsOption
            };

            var parser = new GnuOptionSetParser(
                allOptions, filesOption, filesOption.Forms[0]);

            parsedOptions = parser.Parse(args, log);

            if (!parsedOptions.GetValue<bool>(syntaxOnlyFlag))
            {
                log.Log(
                    new LogEntry(
                        Severity.Info,
                        new BulletedList(
                            allOptions
                                .Select<Option, MarkupNode>(TypesetParsedOption)
                                .ToArray<MarkupNode>(),
                            true)));
            }
        }

        private static MarkupNode TypesetParsedOption(Option opt)
        {
            return new Sequence(
                new MarkupNode[]
                {
                    new DecorationSpan(new Text(opt.Forms[0].ToString()), TextDecoration.Bold),
                    new Text(": "),
                    new Text(ValueToString(parsedOptions.GetValue<object>(opt)))
                });
        }

        private static string ValueToString(object value)
        {
            if (value is IReadOnlyList<object>)
            {
                var sb = new StringBuilder();
                sb.Append("{ ");
                var arr = (IReadOnlyList<object>)value;
                for (int i = 0; i < arr.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ValueToString(arr[i]));
                }
                sb.Append(" }");
                return sb.ToString();
            }
            else
            {
                return value.ToString();
            }
        }

        private static LogEntry MakeDiagnostic(LogEntry entry)
        {
            return DiagnosticExtractor
                .Transform(entry, new Text("program"))
                .Map(WrapBox.WordWrap);
        }
    }
}
