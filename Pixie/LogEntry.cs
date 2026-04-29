using System;
using Pixie.Markup;

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
        public LogEntry(Severity severity, Block contents)
        {
            this = default(LogEntry);
            this.Severity = severity;
            this.Contents = contents;
        }

        /// <summary>
        /// Creates a log entry with the given severity, title and contents.
        /// </summary>
        /// <param name="severity">The log entry's severity.</param>
        /// <param name="title">The log entry's title.</param>
        /// <param name="contents">The log entry's contents.</param>
        public LogEntry(
            Severity severity,
            string title,
            params Block[] contents)
        {
            this = default(LogEntry);
            this.Severity = severity;

            var extendedContents = new Block[contents.Length + 1];
            extendedContents[0] = new Title(title);
            Array.Copy(contents, 0, extendedContents, 1, contents.Length);
            this.Contents = new Stack(extendedContents);
        }

        /// <summary>
        /// Creates a log entry with the given severity and contents.
        /// </summary>
        /// <param name="severity">The log entry's severity.</param>
        /// <param name="contents">The log entry's contents.</param>
        public LogEntry(Severity severity, params Block[] contents)
        {
            this = default(LogEntry);
            this.Severity = severity;
            this.Contents = new Stack(contents);
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
        public Block Contents { get; private set; }

        /// <summary>
        /// Creates a new log entry by applying a mapping to this
        /// log entry's contents.
        /// </summary>
        /// <param name="mapping">The mapping to apply.</param>
        /// <returns>A new log entry.</returns>
        public LogEntry Map(Func<Block, Block> mapping)
        {
            return new LogEntry(Severity, mapping(Contents));
        }
    }
}
