using System.Collections.Generic;

namespace Pixie
{
    /// <summary>
    /// A log implementation that records all incoming entries
    /// and forwards them to another log.
    /// </summary>
    public sealed class RecordingLog : ILog
    {
        /// <summary>
        /// Creates a log that simply records all incoming messages.
        /// </summary>
        public RecordingLog()
            : this(NullLog.Instance)
        { }

        /// <summary>
        /// Creates a log that forwards all of its incoming messages
        /// to a particular log and also records them.
        /// </summary>
        /// <param name="forwardingLog">
        /// The log to forward messages to.
        /// </param>
        public RecordingLog(ILog forwardingLog)
        {
            this.recorded = new List<LogEntry>();
            this.ForwardingLog = forwardingLog;
        }

        private List<LogEntry> recorded;

        /// <summary>
        /// Gets the log to which messages are sent by this log
        /// after they have been recorded.
        /// </summary>
        /// <returns>The inner log.</returns>
        public ILog ForwardingLog { get; private set; }

        /// <summary>
        /// Gets a list of all entries that have been recorded
        /// by this log.
        /// </summary>
        /// <returns>A list of all recorded entries.</returns>
        public IReadOnlyList<LogEntry> RecordedEntries => recorded;

        /// <summary>
        /// Tests if this log contains at least one entry of
        /// a particular severity.
        /// </summary>
        /// <param name="severity">The severity to look for.</param>
        /// <returns>
        /// <c>true</c> if this log contains at least one entry
        /// of <paramref name="severity"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Severity severity)
        {
            int entryCount = recorded.Count;
            for (int i = 0; i < entryCount; i++)
            {
                if (recorded[i].Severity == severity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public void Log(LogEntry entry)
        {
            lock (recorded)
            {
                recorded.Add(entry);
            }
            ForwardingLog.Log(entry);
        }
    }
}
