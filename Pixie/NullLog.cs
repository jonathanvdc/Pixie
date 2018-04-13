namespace Pixie
{
    /// <summary>
    /// A log implementation that discards all incoming entries.
    /// </summary>
    public sealed class NullLog : ILog
    {
        private NullLog()
        { }

        /// <summary>
        /// A log that discards all incoming entries.
        /// </summary>
        /// <returns>A null log.</returns>
        public static readonly NullLog Instance = new NullLog();

        /// <inheritdoc/>
        public void Log(LogEntry entry)
        { }
    }
}