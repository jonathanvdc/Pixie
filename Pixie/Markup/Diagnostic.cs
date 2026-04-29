namespace Pixie.Markup
{
    /// <summary>
    /// Describes a compiler-style diagnostic with an inline header, an optional
    /// inline message, and optional block details.
    /// </summary>
    public sealed class Diagnostic : Block
    {
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
            switch (severity)
            {
                case Severity.Info:
                    return "info";
                case Severity.Message:
                    return "message";
                case Severity.Warning:
                    return "warning";
                default:
                    return "error";
            }
        }

        private static Color GetThemeColor(Severity severity)
        {
            switch (severity)
            {
                case Severity.Warning:
                    return Colors.Yellow;
                case Severity.Error:
                    return Colors.Red;
                default:
                    return Colors.Green;
            }
        }

        public Inline Origin { get; private set; }

        public string Kind { get; private set; }

        public Color ThemeColor { get; private set; }

        public Inline Title { get; private set; }

        public Inline Message { get; private set; }

        public Block Details { get; private set; }
    }
}
