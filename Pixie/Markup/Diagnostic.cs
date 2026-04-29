namespace Pixie.Markup
{
    /// <summary>
    /// Describes a compiler-style diagnostic with an inline header, an optional
    /// inline message, and optional block details.
    /// </summary>
    public sealed class Diagnostic : Block
    {
        /// <summary>
        /// Creates a compiler-style diagnostic.
        /// </summary>
        /// <param name="origin">The diagnostic origin, such as a source location.</param>
        /// <param name="kind">The diagnostic kind label.</param>
        /// <param name="themeColor">The diagnostic theme color.</param>
        /// <param name="title">The diagnostic title.</param>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="details">Additional block details for the diagnostic.</param>
        public Diagnostic(
            Inline origin,
            string kind,
            Color themeColor,
            Inline title,
            Inline message,
            Block details)
        {
            this.Origin = origin;
            this.Kind = kind;
            this.ThemeColor = themeColor;
            this.Title = title;
            this.Message = message;
            this.Details = details;
        }

        /// <summary>
        /// Creates a diagnostic from a severity level.
        /// </summary>
        /// <param name="severity">The diagnostic severity.</param>
        /// <param name="origin">The diagnostic origin, such as a source location.</param>
        /// <param name="title">The diagnostic title.</param>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="details">Additional block details for the diagnostic.</param>
        /// <returns>A diagnostic configured for the given severity.</returns>
        public static Diagnostic FromSeverity(
            Severity severity,
            Inline origin,
            Inline title,
            Inline message,
            Block details)
        {
            return new Diagnostic(
                origin,
                GetKind(severity),
                GetThemeColor(severity),
                title,
                message,
                details);
        }

        private static string GetKind(Severity severity)
        {
            return severity switch
            {
                Severity.Info => "info",
                Severity.Message => "message",
                Severity.Warning => "warning",
                _ => "error",
            };
        }

        private static Color GetThemeColor(Severity severity)
        {
            return severity switch
            {
                Severity.Warning => Colors.Yellow,
                Severity.Error => Colors.Red,
                _ => Colors.Green,
            };
        }

        /// <summary>
        /// Gets the diagnostic origin, such as a source location.
        /// </summary>
        public Inline Origin { get; private set; }

        /// <summary>
        /// Gets the diagnostic kind label.
        /// </summary>
        public string Kind { get; private set; }

        /// <summary>
        /// Gets the diagnostic theme color.
        /// </summary>
        public Color ThemeColor { get; private set; }

        /// <summary>
        /// Gets the diagnostic title.
        /// </summary>
        public Inline Title { get; private set; }

        /// <summary>
        /// Gets the diagnostic message.
        /// </summary>
        public Inline Message { get; private set; }

        /// <summary>
        /// Gets additional block details for the diagnostic.
        /// </summary>
        public Block Details { get; private set; }
    }
}
