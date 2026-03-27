using NUnit.Framework;
using Pixie.Markup;
using Pixie.Options;

namespace Pixie.Tests
{
    [TestFixture]
    public class RenderingBehaviorTests
    {
        [Test]
        public void PrefixBoxAlignsContinuationLinesWithPrefix()
        {
            var rendered = RenderTests.Render(new PrefixBox("> ", new Paragraph("alpha\nbeta"))).Trim();

            StringAssert.StartsWith("> ", rendered);
            StringAssert.Contains("alpha", rendered);
            StringAssert.Contains("beta", rendered);
        }

        [Test]
        public void ParagraphSeparatesItsContentsFromNeighbors()
        {
            var rendered = RenderTests.Render(new Sequence("before", new Paragraph("middle"), "after")).Trim();

            StringAssert.Contains("before", rendered);
            StringAssert.Contains("middle", rendered);
            StringAssert.Contains("after", rendered);
            StringAssert.Contains("\n\nmiddle\n\n", rendered);
        }

        [Test]
        public void ColorAndDecorationSpansFallBackToContents()
        {
            RenderTests.AssertRendersAs(
                new Sequence(
                    new ColorSpan("red", Colors.Red),
                    " ",
                    DecorationSpan.MakeBold("bold")),
                "red bold");
        }

        [Test]
        public void DiagnosticFallbackIncludesOriginKindAndTitle()
        {
            var diagnostic = new Diagnostic("pixie", "error", Colors.Red, "bad flag", "details");

            var rendered = RenderTests.Render(diagnostic).Trim();

            StringAssert.Contains("pixie: error: bad flag", rendered);
            StringAssert.Contains("details", rendered);
        }

        [Test]
        public void OptionHelpUsesPrefixLayoutForShortFlag()
        {
            var option = new FlagOption(OptionForm.Short("c")).WithDescription("compile only");

            var rendered = RenderTests.Render(new OptionHelp(option, GnuOptionPrinter.Instance)).Trim();

            StringAssert.Contains("-c", rendered);
            StringAssert.Contains("compile only", rendered);
        }
    }
}
