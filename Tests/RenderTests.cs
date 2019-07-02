using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Terminal;
using Pixie.Terminal.Devices;
using Pixie.Terminal.Render;

namespace Pixie.Tests
{
    [TestFixture]
    public class RenderTests
    {
        /// <summary>
        /// Renders a markup node as a string using a terminal log.
        /// </summary>
        /// <param name="node">The node to render.</param>
        /// <param name="extraRenderers">A sequence of additional renderers to use.</param>
        /// <returns>A rendered node.</returns>
        public static string Render(MarkupNode node, params NodeRenderer[] extraRenderers)
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var log = new TerminalLog(terminal).WithRenderers(extraRenderers);
            log.Log(node);
            return writer.ToString();
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
        /// <param name="extraRenderers">A sequence of additional renderers to use.</param>
        public static void AssertRendersAs(
            MarkupNode node,
            string expected,
            params NodeRenderer[] extraRenderers)
        {
            Assert.AreEqual(
                PrepareForComparison(expected),
                PrepareForComparison(Render(node, extraRenderers)));
        }

        [Test]
        public void TextBeforeBox()
        {
            AssertRendersAs(
                new Sequence(
                    new MarkupNode[]
                    {
                        "I did not hit her. I did naaahhht.",
                        new Box("Oh hi Mark")
                    }),
                "I did not hit her. I did naaahhht.\nOh hi Mark");
        }

        [Test]
        public void TextBeforeWrapBox()
        {
            AssertRendersAs(
                new Sequence(
                    new MarkupNode[]
                    {
                        "I did not hit her. I did naaahhht.",
                        new WrapBox("Oh hi Mark", WrappingStrategy.Word)
                    }),
                "I did not hit her. I did naaahhht.\nOh hi Mark");
        }

        [Test]
        public void TextBeforeNestedWrapBox()
        {
            AssertRendersAs(
                new WrapBox(
                    new Sequence(
                        new MarkupNode[]
                        {
                            "I did not hit her. I did naaahhht.",
                            new WrapBox("Oh hi Mark", WrappingStrategy.Word)
                        }),
                    WrappingStrategy.Word),
                "I did not hit her. I did naaahhht.\nOh hi Mark");
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
  9 | }",
                new HighlightedSourceRenderer(5));
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
  10 | }",
                new HighlightedSourceRenderer(5));
        }
    }
}
