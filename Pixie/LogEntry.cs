namespace Pixie
{
    /// <summary>
    /// Describes a log entry: a self-contained nugget of information.
    /// </summary>
    public struct LogEntry
    {
        /// <summary>
        /// Creates a log entry with the given severity and contents.
        /// </summary>
        /// <param name="severity">The log entry's severity.</param>
        /// <param name="contents">The log entry's contents.</param>
        public LogEntry(Severity severity, MarkupNode contents)
        {
            this = default(LogEntry);
            this.Severity = severity;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the severity for this log entry.
        /// </summary>
        /// <returns>The log entry's severity.</returns>
        public Severity Severity { get; private set; }

        /// <summary>
        /// Gets the markup nodes that make up this log entry's contents.
        /// </summary>
        /// <returns>The log entry's contents.</returns>
        public MarkupNode Contents { get; private set; }
    }
}