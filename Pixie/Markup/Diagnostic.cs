namespace Pixie.Markup
{
    /// <summary>
    /// Describes a diagnostic as issued by typical command-line tools:
    /// a self-contained nugget of information to the user.
    /// </summary>
    public sealed class Diagnostic : MarkupNode
    {
        /// <summary>
        /// Creates a diagnostic.
        /// </summary>
        /// <param name="origin">
        /// The origin of the diagnostic: the line of code
        /// or application that causes the diagnostic to be
        /// issued.
        /// </param>
        /// <param name="kind">
        /// A single-word description of the kind of diagnostic
        /// to create.
        /// </param>
        /// <param name="themeColor">
        /// The diagnostic's theme color.
        /// </param>
        /// <param name="title">
        /// The contents of the diagnostic's title.
        /// </param>
        /// <param name="message"></param>
        public Diagnostic(
            MarkupNode origin,
            string kind,
            Color themeColor,
            MarkupNode title,
            MarkupNode message)
        {
            this.Origin = origin;
            this.Kind = kind;
            this.ThemeColor = themeColor;
            this.Title = title;
            this.Message = message;
        }

        /// <summary>
        /// Gets the origin of this diagnostic: the line of code
        /// or application that causes the diagnostic to be
        /// issued.
        /// </summary>
        /// <returns>The origin of the diagnostic.</returns>
        public MarkupNode Origin { get; private set; }

        /// <summary>
        /// Gets a (single-word) description of the kind of diagnostic
        /// this instance is.
        /// </summary>
        /// <returns>The kind of diagnostic.</returns>
        public string Kind { get; private set; }

        /// <summary>
        /// Gets this diagnostic's theme color.
        /// </summary>
        /// <returns>The theme color.</returns>
        public Color ThemeColor { get; private set; }

        /// <summary>
        /// Gets the contents of this diagnostic's title.
        /// </summary>
        /// <returns>The title.</returns>
        public MarkupNode Title { get; private set; }

        /// <summary>
        /// Gets this diagnostic's message.
        /// </summary>
        /// <returns>The message node.</returns>
        public MarkupNode Message { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback
        {
            get
            {
                var header =
                    new Sequence(
                        Origin,
                        new Text(": "),
                        new ColorSpan(
                            new Text(Kind + ": "),
                            ThemeColor),
                        Title,
                        new Text(": "));

                return new Sequence(
                    new DecorationSpan(header, TextDecoration.Bold),
                    Message);
            }
        }
    }
}