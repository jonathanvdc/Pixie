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
        private static readonly FlagOption syntaxOnlyFlag = new FlagOption(
            OptionForm.Short("fsyntax-only"),
            OptionForm.Short("fno-syntax-only"),
            false,
            "Check the code for syntax errors only.");

        private static readonly SequenceOption<string> filesOption = SequenceOption.CreateStringOption(
            OptionForm.Long("files"),
            "Chooses input files.");

        private static readonly ValueOption<int> optimizeOption = ValueOption.CreateInt32Option(
            OptionForm.Short("O"),
            0,
            "Pick an optimization level.");

        private static readonly FlagOption optimizeFastFlag = new FlagOption(
            OptionForm.Short("Ofast"),
            "Enable aggressive optimizations.");

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
                optimizeFastFlag
            };

            var parser = new GnuOptionSetParser(
                allOptions, filesOption, filesOption.Forms[0]);

            parsedOptions = parser.Parse(args, log);

            log.Log(
                new LogEntry(
                    Severity.Info,
                    new BulletedList(
                        allOptions
                            .Select<Option, MarkupNode>(TypesetParsedOption)
                            .ToArray<MarkupNode>(),
                        true)));
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
            return DiagnosticExtractor.Transform(entry, new Text("program"));
        }
    }
}
