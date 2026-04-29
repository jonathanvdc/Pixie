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
            //
            ILog log = TerminalLog.AcquireStandardOutput();

            var doc = new StringDocument("code.cs", SourceCode);
            var ctorStartOffset = SourceCode.IndexOf("public Program()");
            var ctorNameOffset = SourceCode.IndexOf("Program()");

            var highlightRegion = new SourceRegion(
                new SourceSpan(doc, ctorStartOffset, "public Program()".Length))
                .ExcludeCharacters(char.IsWhiteSpace);

            var focusRegion = new SourceRegion(
                new SourceSpan(doc, ctorNameOffset, "Program".Length));

            var highlightedSource = new HighlightedSource(highlightRegion, focusRegion);
            log.ErrorDiagnostic(
                new SourceReference(highlightedSource.HighlightedSpan),
                "hello world",
                new Text("look at this beautiful error message!"),
                highlightedSource);
        }
        private const string SourceCode = @"public static class Program
{
    public Program()
    { }
}";
    }
}
