using System;
using System.Collections.Generic;

namespace Pixie
{
    /// <summary>
    /// A log implementation that applies a sequence of transformations
    /// to log entries before sending them to another log.
    /// </summary>
    public sealed class TransformLog : ILog
    {
        /// <summary>
        /// Creates a log that applies a sequence of transformations
        /// in order to each log entry before sending it to another
        /// log.
        /// </summary>
        /// <param name="outputLog">A log to which output is sent.</param>
        /// <param name="transform">The transform to apply to each log entry.</param>
        public TransformLog(
            ILog outputLog,
            Func<LogEntry, LogEntry> transform)
            : this(
                outputLog,
                new Func<LogEntry, LogEntry>[] { transform })
        { }

        /// <summary>
        /// Creates a log that applies a sequence of transformations
        /// in order to each log entry before sending it to another
        /// log.
        /// </summary>
        /// <param name="outputLog">A log to which output is sent.</param>
        /// <param name="transforms">
        /// A list of transformations which are to be applied to each log
        /// entry in order.
        /// </param>
        public TransformLog(
            ILog outputLog,
            params Func<LogEntry, LogEntry>[] transforms)
            : this(
                outputLog,
                (IReadOnlyList<Func<LogEntry, LogEntry>>)transforms)
        { }

        /// <summary>
        /// Creates a log that applies a sequence of transformations
        /// in order to each log entry before sending it to another
        /// log.
        /// </summary>
        /// <param name="outputLog">A log to which output is sent.</param>
        /// <param name="transforms">
        /// A list of transformations which are to be applied to each log
        /// entry in order.
        /// </param>
        public TransformLog(
            ILog outputLog,
            IReadOnlyList<Func<LogEntry, LogEntry>> transforms)
        {
            this.OutputLog = outputLog;
            this.transforms = transforms;
        }

        /// <summary>
        /// Gets the log to which this transformation log
        /// sends transformed entries.
        /// </summary>
        /// <returns>The output log.</returns>
        public ILog OutputLog { get; private set; }

        private IReadOnlyList<Func<LogEntry, LogEntry>> transforms;

        /// <inheritdoc/>
        public void Log(LogEntry entry)
        {
            var transformCount = transforms.Count;
            for (int i = 0; i < transformCount; i++)
            {
                entry = transforms[i](entry);
            }
            OutputLog.Log(entry);
        }
    }
}