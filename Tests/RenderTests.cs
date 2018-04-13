using System.IO;
using NUnit.Framework;
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
        public static string Render(MarkupNode node)
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80);
            var log = new TerminalLog(terminal);
            log.Log(node);
            return writer.ToString().Trim().Replace("\r", "");
        }

        /// <summary>
        /// Checks the a node renders as expected.
        /// </summary>
        /// <param name="node">The node to render.</param>
        /// <param name="expected">The expected output.</param>
        public static void AssertRendersAs(MarkupNode node, string expected)
        {
            Assert.AreEqual(expected, Render(node));
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
    }
}
