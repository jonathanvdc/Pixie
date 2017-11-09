namespace Pixie
{
    /// <summary>
    /// Defines common functionality for logs that accept entries.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs the given entry in this log.
        /// </summary>
        /// <param name="entry">The entry to log.</param>
        void Log(LogEntry entry);
    }
}