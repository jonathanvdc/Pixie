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
        /// <see cref="WrapBox.WordWrap(MarkupNode)"/> before rendering.
        /// </summary>
        /// <param name="log">The log to wrap.</param>
        /// <returns>A log that word-wraps each entry.</returns>
        public static ILog WithWordWrap(this ILog log)
        {
            return log.WithTransform(entry => entry.Map(WrapBox.WordWrap));
        }

        /// <summary>
        /// Wraps a log so that every entry is first converted to a
        /// compiler-style diagnostic.
        /// This is the easiest way to get headers such as
        /// <c>file.cs:12:4: error: expected expression</c> without calling
        /// <see cref="DiagnosticExtractor.Transform(LogEntry, MarkupNode)"/>
        /// at each call site.
        /// </summary>
        /// <remarks>
        /// This is typically combined with <c>TerminalLog.Acquire()</c> once at
        /// application startup, after which callers can log ordinary
        /// <see cref="LogEntry"/> values and still get diagnostic headers.
        /// </remarks>
        /// <param name="log">The log to wrap.</param>
        /// <param name="defaultOrigin">
        /// The fallback origin to use when a log entry does not already
        /// contain a source reference.
        /// </param>
        /// <returns>A log that emits diagnostic-formatted entries.</returns>
        public static ILog WithDiagnostics(this ILog log, MarkupNode defaultOrigin)
        {
            return log.WithTransform(
                entry => DiagnosticExtractor.Transform(entry, defaultOrigin));
        }

        /// <summary>
        /// Wraps a log so that every entry is first converted to a
        /// compiler-style diagnostic.
        /// </summary>
        /// <remarks>
        /// This overload is a convenience for simple applications that want a
        /// plain-text fallback origin such as an executable name.
        /// </remarks>
        /// <param name="log">The log to wrap.</param>
        /// <param name="defaultOrigin">
        /// The fallback origin to use when a log entry does not already
        /// contain a source reference.
        /// </param>
        /// <returns>A log that emits diagnostic-formatted entries.</returns>
        public static ILog WithDiagnostics(this ILog log, string defaultOrigin)
        {
            return log.WithDiagnostics(new Text(defaultOrigin));
        }
    }
}
