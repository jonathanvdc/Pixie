using System.Collections.Generic;

namespace Pixie
{
    /// <summary>
    /// A type of log that sends messages to another log and aborts
    /// execution if the severity of an error is deemed fatal
    /// </summary>
    public sealed class TestLog : ILog
    {
        /// <summary>
        /// Creates a test log.
        /// </summary>
        /// <param name="fatalSeverities">
        /// A sequence of all severities that are considered fatal.
        /// A log entry whose severity is fatal triggers an exception.
        /// </param>
        /// <param name="redirectionLog">
        /// A log to which messages are sent before the decision to
        /// throw an exception or not is taken.
        /// </param>
        public TestLog(
            IEnumerable<Severity> fatalSeverities,
            ILog redirectionLog)
        {
            this.fatalSeveritySet = new HashSet<Severity>(fatalSeverities);
            this.RedirectionLog = redirectionLog;
        }

        private HashSet<Severity> fatalSeveritySet;

        /// <summary>
        /// Gets the set of all severities that are considered fatal
        /// by this log. A log entry whose severity is fatal triggers
        /// an exception.
        /// </summary>
        /// <returns>A sequence of severities.</returns>
        public IEnumerable<Severity> FatalSeverities => fatalSeveritySet;

        /// <summary>
        /// Gets the log to which messages are sent by this log before
        /// the decision to abort the program or not is taken.
        /// </summary>
        /// <returns>The inner log.</returns>
        public ILog RedirectionLog { get; private set; }

        /// <inheritdoc/>
        public void Log(LogEntry entry)
        {
            RedirectionLog.Log(entry);
            if (fatalSeveritySet.Contains(entry.Severity))
            {
                throw new PixieException();
            }
        }
    }
}
