using NUnit.Framework;

namespace Pixie.Tests
{
    [TestFixture]
    public class RecordingLogTests
    {
        [Test]
        public void RecordWithoutForwarding()
        {
            var entry1 = new LogEntry(Severity.Message, "Oh hi there!");
            var entry2 = new LogEntry(Severity.Error, "Please don't do that.");

            var log = new RecordingLog();
            log.Log(entry1);
            log.Log(entry2);

            Assert.AreEqual(log.RecordedEntries.Count, 2);
            Assert.AreEqual(log.RecordedEntries[0].Severity, Severity.Message);
            Assert.AreEqual(log.RecordedEntries[1].Severity, Severity.Error);
            Assert.IsTrue(log.Contains(Severity.Error));
            Assert.IsFalse(log.Contains(Severity.Warning));
            Assert.IsTrue(log.Contains(Severity.Message));
            Assert.IsFalse(log.Contains(Severity.Info));
        }

        [Test]
        public void RecordAndForward()
        {
            var entry1 = new LogEntry(Severity.Message, "Oh hi there!");
            var entry2 = new LogEntry(Severity.Error, "Please don't do that.");
            var entry3 = new LogEntry(Severity.Warning, "Whoa. That's dangerous.");

            var log1 = new RecordingLog();
            var log2 = new RecordingLog(log1);
            log2.Log(entry1);
            log1.Log(entry2);
            log2.Log(entry3);

            Assert.AreEqual(log2.RecordedEntries.Count, 2);
            Assert.AreEqual(log2.RecordedEntries[0].Severity, Severity.Message);
            Assert.AreEqual(log2.RecordedEntries[1].Severity, Severity.Warning);
            Assert.IsFalse(log2.Contains(Severity.Error));
            Assert.IsTrue(log2.Contains(Severity.Warning));
            Assert.IsTrue(log2.Contains(Severity.Message));
            Assert.IsFalse(log2.Contains(Severity.Info));

            Assert.AreEqual(log1.RecordedEntries.Count, 3);
            Assert.AreEqual(log1.RecordedEntries[0].Severity, Severity.Message);
            Assert.AreEqual(log1.RecordedEntries[1].Severity, Severity.Error);
            Assert.AreEqual(log1.RecordedEntries[2].Severity, Severity.Warning);
            Assert.IsTrue(log1.Contains(Severity.Error));
            Assert.IsTrue(log1.Contains(Severity.Warning));
            Assert.IsTrue(log1.Contains(Severity.Message));
            Assert.IsFalse(log1.Contains(Severity.Info));
        }
    }
}
