using System;
using System.Linq;
using Pixie;
using Pixie.Terminal;
using Pixie.Markup;
using Pixie.Code;

namespace CaretDiagnostics
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // First, acquire a terminal log. You should acquire
            // a log once and then re-use it in your application.
            var log = TerminalLog.Acquire();

            var doc = new StringDocument("code.cs", SourceCode);
            var ctorStartOffset = SourceCode.IndexOf("public Program()");
            var ctorRegion = new SourceRegion(new SourceSpan(doc, ctorStartOffset, "public Program()".Length));

            // Write an entry to the log that contains the things
            // we would like to print.
            log.Log(
                new LogEntry(
                    Severity.Info,
                    new MarkupNode[]
                    {
                        new Title(new ColorSpan(new Text("Hello world"), Colors.Green)),
                        new HighlightedSource(ctorRegion)
                    }));
        }

        private const string SourceCode = @"public static class Program
{
    public Program()
    { }
}";
    }
}
