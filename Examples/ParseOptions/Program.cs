using System;
using System.Linq;
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
            false);

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
                syntaxOnlyFlag
            };

            var parser = new GnuOptionSetParser(
                allOptions, syntaxOnlyFlag, syntaxOnlyFlag.PositiveForms[0]);

            parsedOptions = parser.Parse(args, log);

            foreach (var item in allOptions)
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
                new DecorationSpan(new Text(opt.Forms[0].ToString()), TextDecoration.Bold),
                new Text(": "),
                new Text(parsedOptions.GetValue<object>(opt).ToString()));
        }

        private static LogEntry MakeDiagnostic(LogEntry entry)
        {
            return DiagnosticExtractor.Transform(entry, new Text("program"));
        }
    }
}
