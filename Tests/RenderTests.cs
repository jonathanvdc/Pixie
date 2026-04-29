using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Terminal;
using Pixie.Terminal.Devices;

namespace Pixie.Tests
{
    [TestFixture]
    public class RenderTests
    {
        /// <summary>
        /// Renders a markup node as a string using a terminal log.
        /// </summary>
        /// <param name="node">The node to render.</param>
        /// <returns>A rendered node.</returns>
        public static string Render(Block node)
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var log = new TerminalLog(terminal);
            log.Log(node);
            return writer.ToString();
        }

        public static string Render(Inline node)
        {
            return Render(new Paragraph(node));
        }

        private static string PrepareForComparison(string value)
        {
            return value.Trim().Replace("\r", "");
        }

        /// <summary>
        /// Checks that a node renders as expected.
        /// </summary>
        /// <param name="node">The node to render.</param>
        /// <param name="expected">The expected output.</param>
        public static void AssertRendersAs(
            Block node,
            string expected)
        {
            Assert.AreEqual(
                PrepareForComparison(expected),
                PrepareForComparison(Render(node)));
        }

        public static void AssertRendersAs(
            Inline node,
            string expected)
        {
            Assert.AreEqual(
                PrepareForComparison(expected),
                PrepareForComparison(Render(node)));
        }

        [Test]
        public void TerminalLogDoesNotPrefixFirstEntryWithBlankLine()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var log = new TerminalLog(terminal);

            log.Info("Hello from Pixie.");

            Assert.IsFalse(writer.ToString().StartsWith("\n", StringComparison.Ordinal));
            StringAssert.Contains("Hello from Pixie.", writer.ToString());
        }

        [Test]
        public void TerminalLogTerminatesEachEntryWithNewline()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var log = new TerminalLog(terminal);

            log.Info("Hello from Pixie.");

            Assert.IsTrue(writer.ToString().EndsWith("\n", StringComparison.Ordinal));
        }

        [Test]
        public void TerminalLogDoesNotDuplicateExistingEntryNewline()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var log = new TerminalLog(terminal);

            log.Info(new Paragraph("Hello from Pixie.", NewLine.Instance));

            Assert.AreEqual("Hello from Pixie.\n", writer.ToString());
        }

        [Test]
        public void TextBeforeBox()
        {
            AssertRendersAs(
                new Stack(
                    new Paragraph("I did not hit her. I did naaahhht."),
                    new Paragraph("Oh hi Mark")),
                "I did not hit her. I did naaahhht.\n\nOh hi Mark");
        }

        [Test]
        public void TextBeforeWrapBox()
        {
            AssertRendersAs(
                new Stack(
                    new Paragraph("I did not hit her. I did naaahhht."),
                    new WrapBox(new Paragraph("Oh hi Mark"), WrappingStrategy.Word)),
                "I did not hit her. I did naaahhht.\n\nOh hi Mark");
        }

        [Test]
        public void TextBeforeNestedWrapBox()
        {
            AssertRendersAs(
                new WrapBox(
                    new Stack(
                        new Paragraph("I did not hit her. I did naaahhht."),
                        new WrapBox(new Paragraph("Oh hi Mark"), WrappingStrategy.Word)),
                    WrappingStrategy.Word),
                "I did not hit her. I did naaahhht.\n\nOh hi Mark");
        }

        [Test]
        public void SourceCodeEmptyLineNumbering()
        {
            // This is a regression test for #1 (https://github.com/jonathanvdc/Pixie/issues/1)
            // It checks that empty lines are properly rendered.

            var source = @"public static class Program
{

    public Program()
    {

    }

}";

            var doc = new StringDocument("code.cs", source);
            var ctorStartOffset = source.IndexOf("public Program()", StringComparison.InvariantCulture);
            var ctorNameOffset = source.IndexOf("Program()", StringComparison.InvariantCulture);

            var highlightRegion = new SourceRegion(
                    new SourceSpan(doc, ctorStartOffset, "public Program()".Length))
                .ExcludeCharacters(char.IsWhiteSpace);

            var focusRegion = new SourceRegion(
                new SourceSpan(doc, ctorNameOffset, "Program".Length));

            AssertRendersAs(
                new HighlightedSource(highlightRegion, focusRegion),
                @"
  1 | public static class Program
  2 | {
  3 | 
  4 |     public Program()
    |     ~~~~~~ ^~~~~~~~~
  5 |     {
  6 | 
  7 |     }
  8 | 
  9 | }");
        }

        [Test]
        public void SourceCodeRightAlignNumbering()
        {
            // This test checks that source code line numbering is right-aligned.

            var source = @"using System;
public static class Program
{

    public Program()
    {

    }

}";

            var doc = new StringDocument("code.cs", source);
            var ctorStartOffset = source.IndexOf("public Program()", StringComparison.InvariantCulture);
            var ctorNameOffset = source.IndexOf("Program()", StringComparison.InvariantCulture);

            var highlightRegion = new SourceRegion(
                    new SourceSpan(doc, ctorStartOffset, "public Program()".Length))
                .ExcludeCharacters(char.IsWhiteSpace);

            var focusRegion = new SourceRegion(
                new SourceSpan(doc, ctorNameOffset, "Program".Length));

            AssertRendersAs(
                new HighlightedSource(highlightRegion, focusRegion),
                @"
   1 | using System;
   2 | public static class Program
   3 | {
   4 | 
   5 |     public Program()
     |     ~~~~~~ ^~~~~~~~~
   6 |     {
   7 | 
   8 |     }
   9 | 
  10 | }");
        }

        [Test]
        public void PiecewiseSourceDiagnosticsRenderOriginalSource()
        {
            var source = "before\nerror here\nafter";
            var original = new StringDocument("input.txt", source);
            var generatedPrefix = "// generated\n";
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddText(generatedPrefix)
                .AddSource(new SourceSpan(original, 0, original.Length))
                .Build();

            int errorOffset = generatedPrefix.Length
                + source.IndexOf("error", StringComparison.InvariantCulture);
            var focusRegion = new SourceRegion(new SourceSpan(document, errorOffset, "error".Length));

            AssertRendersAs(
                new HighlightedSource(focusRegion, focusRegion),
                @"
  1 | before
  2 | error here
    | ^~~~~
  3 | after");
        }
    }
}
