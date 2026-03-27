using NUnit.Framework;

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
    }
}
