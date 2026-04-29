using Pixie.Markup;

namespace Pixie
{
    /// <summary>
    /// Convenience helpers for creating and logging entries with a specific severity.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs an informational entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Info(this ILog log, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Info, contents));
        }

        /// <summary>
        /// Logs an informational entry with a title.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="title">The entry title.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Info(this ILog log, string title, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Info, title, contents));
        }

        /// <summary>
        /// Logs a plain message entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Message(this ILog log, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Message, contents));
        }

        /// <summary>
        /// Logs a plain message entry with a title.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="title">The entry title.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Message(this ILog log, string title, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Message, title, contents));
        }

        /// <summary>
        /// Logs a warning entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Warning(this ILog log, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Warning, contents));
        }

        /// <summary>
        /// Logs a warning entry with a title.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="title">The entry title.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Warning(this ILog log, string title, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Warning, title, contents));
        }

        /// <summary>
        /// Logs an error entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Error(this ILog log, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Error, contents));
        }

        /// <summary>
        /// Logs an error entry with a title.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="title">The entry title.</param>
        /// <param name="contents">The entry contents.</param>
        public static void Error(this ILog log, string title, params Block[] contents)
        {
            log.Log(new LogEntry(Severity.Error, title, contents));
        }

        /// <summary>
        /// Logs a compiler-style diagnostic entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="severity">The diagnostic severity.</param>
        /// <param name="origin">The diagnostic origin, such as a source location.</param>
        /// <param name="title">The diagnostic title.</param>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="details">Optional additional diagnostic details.</param>
        public static void Diagnostic(
            this ILog log,
            Severity severity,
            Inline origin,
            Inline title,
            Inline message,
            Block details = null)
        {
            log.Log(
                new LogEntry(
                    severity,
                    Markup.Diagnostic.FromSeverity(
                        severity,
                        origin,
                        title,
                        message,
                        details)));
        }

        /// <summary>
        /// Logs an error as a compiler-style diagnostic entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="origin">The diagnostic origin, such as a source location.</param>
        /// <param name="title">The diagnostic title.</param>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="details">Optional additional diagnostic details.</param>
        public static void ErrorDiagnostic(
            this ILog log,
            Inline origin,
            Inline title,
            Inline message,
            Block details = null)
        {
            log.Diagnostic(Severity.Error, origin, title, message, details);
        }

        /// <summary>
        /// Logs a warning as a compiler-style diagnostic entry.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="origin">The diagnostic origin, such as a source location.</param>
        /// <param name="title">The diagnostic title.</param>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="details">Optional additional diagnostic details.</param>
        public static void WarningDiagnostic(
            this ILog log,
            Inline origin,
            Inline title,
            Inline message,
            Block details = null)
        {
            log.Diagnostic(Severity.Warning, origin, title, message, details);
        }
    }
}
