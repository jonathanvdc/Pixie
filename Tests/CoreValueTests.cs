using NUnit.Framework;
using Pixie.Markup;

namespace Pixie.Tests
{
    [TestFixture]
    public class CoreValueTests
    {
        [Test]
        public void ColorSupportsGrayscaleAndStringFormatting()
        {
            var color = new Color(0.2, 0.4, 0.6, 0.5);

            Assert.AreEqual(0.4, color.Grayscale, 1e-9);
            Assert.AreEqual("a:0.5;r:0.2;g:0.4;b:0.6", color.ToString());
        }

        [Test]
        public void ColorOverBlendsAlphaAndChannels()
        {
            var top = new Color(1.0, 0.0, 0.0, 0.5);
            var bottom = new Color(0.0, 0.0, 1.0, 1.0);

            var result = top.Over(bottom);

            Assert.AreEqual(1.0, result.Alpha, 1e-9);
            Assert.AreEqual(0.5, result.Red, 1e-9);
            Assert.AreEqual(0.0, result.Green, 1e-9);
            Assert.AreEqual(0.5, result.Blue, 1e-9);
        }

        [Test]
        public void NamedColorsExposeExpectedChannelValues()
        {
            Assert.AreEqual("a:0;r:0;g:0;b:0", Colors.Transparent.ToString());
            Assert.AreEqual("a:1;r:1;g:1;b:0", Colors.Yellow.ToString());
            Assert.AreEqual("a:1;r:1;g:0;b:1", Colors.Magenta.ToString());
            Assert.AreEqual("a:1;r:0;g:1;b:1", Colors.Cyan.ToString());
        }

        [Test]
        public void LogEntryTitleConstructorPrefixesATitleNode()
        {
            var entry = new LogEntry(Severity.Warning, "bad option", "details");
            var contents = (Sequence)entry.Contents;

            Assert.IsInstanceOf<Title>(contents.Contents[0]);
            Assert.AreEqual("bad option", ((Text)((Title)contents.Contents[0]).Contents).Contents);
            Assert.AreEqual("details", ((Text)contents.Contents[1]).Contents);
        }

        [Test]
        public void LogEntryMapReplacesContentsWithoutChangingSeverity()
        {
            var entry = new LogEntry(Severity.Info, "before");

            var mapped = entry.Map(_ => (MarkupNode)"after");

            Assert.AreEqual(Severity.Info, mapped.Severity);
            Assert.AreEqual("after", ((Text)mapped.Contents).Contents);
        }
    }
}
