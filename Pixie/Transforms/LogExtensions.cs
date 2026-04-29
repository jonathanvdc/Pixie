using Pixie.Markup;

namespace Pixie.Transforms
{
    /// <summary>
    /// Convenience helpers for decorating logs with common transforms.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Wraps a log so that every entry is first transformed before
        /// being forwarded to the underlying log.
        /// </summary>
        /// <param name="log">The log to wrap.</param>
        /// <param name="transform">The transform to apply to each entry.</param>
        /// <returns>A log that applies the requested transform.</returns>
        public static ILog WithTransform(
            this ILog log,
            System.Func<LogEntry, LogEntry> transform)
        {
            return new TransformLog(log, transform);
        }

        /// <summary>
        /// Wraps a log so that every entry's contents are passed through
        /// <see cref="WrapBox.WordWrap(Block)"/> before rendering.
        /// </summary>
        /// <param name="log">The log to wrap.</param>
        /// <returns>A log that word-wraps each entry.</returns>
        public static ILog WithWordWrap(this ILog log)
        {
            return log.WithTransform(entry => entry.Map(WrapBox.WordWrap));
        }

    }
}
