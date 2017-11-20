using System;
using System.Linq;
using Pixie;
using Pixie.Terminal;
using Pixie.Markup;
using Pixie.Code;
using Pixie.Terminal.Render;
using Pixie.Transforms;

namespace CaretDiagnostics
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // First, acquire a terminal log. You should acquire
            // a log once and then re-use it in your application.
            //
            // In this case, we'll also overwrite the default
            // caret diagnostics renderer with a variation that
            // colors its output red by default and tries to print
            // five lines of context.
            //
            // After that, we'll add a transformation to the log
            // that turns all log entries into diagnostics.
            ILog log = TerminalLog
                .Acquire()
                .WithRenderers(new NodeRenderer[]
                {
                    new HighlightedSourceRenderer(Colors.Red, 5)
                });

            log = new TransformLog(
                log,
                new Func<LogEntry, LogEntry>[]
                {
                    MakeDiagnostic
                });

            var doc = new StringDocument("code.cs", SourceCode);
            var ctorStartOffset = SourceCode.IndexOf("public Program()");
            var ctorNameOffset = SourceCode.IndexOf("Program()");

            var highlightRegion = new SourceRegion(
                new SourceSpan(doc, ctorStartOffset, "public Program()".Length))
                .ExcludeCharacters(char.IsWhiteSpace);

            var focusRegion = new SourceRegion(
                new SourceSpan(doc, ctorNameOffset, "Program".Length));

            // Write an entry to the log that contains the things
            // we would like to print.
            log.Log(
                new LogEntry(
                    Severity.Error,
                    "hello world",
                    new MarkupNode[]
                    {
                        new Text("look at this beautiful error message!"),
                        new HighlightedSource(highlightRegion, focusRegion)
                    }));
        }


        private static LogEntry MakeDiagnostic(LogEntry entry)
        {
            return DiagnosticExtractor.Transform(entry, new Text("program"));
        }

        private const string SourceCode = @"public static class Program
{
    public Program()
    { }
}";
    }
}
