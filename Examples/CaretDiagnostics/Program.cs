using System;
using System.Linq;
using Pixie;
using Pixie.Terminal;
using Pixie.Markup;
using Pixie.Code;
using Pixie.Terminal.Render;

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
            // colors its output red and tries to print five lines
            // of context.
            var log = TerminalLog
                .Acquire()
                .WithRenderers(new NodeRenderer[]
                {
                    new HighlightedSourceRenderer(Colors.Red, 5)
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
                    Severity.Info,
                    new MarkupNode[]
                    {
                        new Title(new ColorSpan(new Text("Hello world"), Colors.Green)),
                        new HighlightedSource(highlightRegion, focusRegion)
                    }));
        }

        private const string SourceCode = @"public static class Program
{
    public Program()
    { }
}";
    }
}
