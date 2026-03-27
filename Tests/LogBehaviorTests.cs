using NUnit.Framework;
using Pixie.Markup;
using Pixie.Transforms;

namespace Pixie.Tests
{
    [TestFixture]
    public class LogBehaviorTests
    {
        [Test]
        public void TestLogForwardsBeforeThrowingOnFatalSeverity()
        {
            var sink = new RecordingLog();
            var log = new TestLog(new[] { Severity.Error }, sink);
            var entry = new LogEntry(Severity.Error, "fatal");

            Assert.Throws<PixieException>(() => log.Log(entry));
            Assert.AreEqual(1, sink.RecordedEntries.Count);
            Assert.AreEqual(Severity.Error, sink.RecordedEntries[0].Severity);
        }

        [Test]
        public void TransformLogAppliesTransformsInOrder()
        {
            var sink = new RecordingLog();
            var log = new TransformLog(
                sink,
                entry => new LogEntry(entry.Severity, "first"),
                entry => new LogEntry(entry.Severity, entry.Contents, " second"));

            log.Log(new LogEntry(Severity.Message, "ignored"));

            StringAssert.Contains("first", RenderTests.Render(sink.RecordedEntries[0].Contents));
            StringAssert.Contains("second", RenderTests.Render(sink.RecordedEntries[0].Contents));
        }

        [Test]
        public void WithDiagnosticsWrapsEntriesInDiagnosticTransform()
        {
            var sink = new RecordingLog();
            var log = sink.WithDiagnostics("program");

            log.Log(new LogEntry(Severity.Error, "oops"));

            var rendered = RenderTests.Render(sink.RecordedEntries[0].Contents);
            StringAssert.Contains("program: error: oops", rendered);
        }

        [Test]
        public void WithTransformAppliesSingleEntryTransform()
        {
            var sink = new RecordingLog();
            var log = sink.WithTransform(
                entry => new LogEntry(entry.Severity, "changed"));

            log.Log(new LogEntry(Severity.Message, "ignored"));

            StringAssert.Contains("changed", RenderTests.Render(sink.RecordedEntries[0].Contents));
        }

        [Test]
        public void WithWordWrapMapsEntryContents()
        {
            var sink = new RecordingLog();
            var log = sink.WithWordWrap();

            log.Log(new LogEntry(
                Severity.Message,
                WrapBox.WordWrap("very long line")));

            var rendered = RenderTests.Render(sink.RecordedEntries[0].Contents);
            StringAssert.Contains(System.Environment.NewLine, rendered);
        }
    }
}
