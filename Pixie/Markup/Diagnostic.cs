using System;

namespace Pixie.Markup
{
    /// <summary>
    /// Describes a compiler-style diagnostic such as an error, warning, or
    /// informational message.
    /// A diagnostic combines an origin, a kind, an optional title, and a
    /// message body into a single markup node that can render as a header like
    /// <c>file.cs:12:4: error: expected expression</c> followed by additional
    /// context such as highlighted source code.
    /// </summary>
    public sealed class Diagnostic : MarkupNode
    {
        /// <summary>
        /// Creates a diagnostic with a header and message body.
        /// </summary>
        /// <param name="origin">
        /// The origin of the diagnostic, such as a source location or
        /// application name.
        /// </param>
        /// <param name="kind">
        /// A single-word category such as <c>error</c>, <c>warning</c>, or
        /// <c>info</c>.
        /// </param>
        /// <param name="themeColor">
        /// The color used to emphasize the diagnostic's kind and related
        /// content.
        /// </param>
        /// <param name="title">
        /// The short headline that appears in the diagnostic header after the
        /// kind.
        /// </param>
        /// <param name="message">
        /// The diagnostic's body, which may contain explanatory text,
        /// highlighted source code, or other markup.
        /// </param>
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
        /// Gets the origin of this diagnostic, typically a source reference or
        /// application name that appears at the start of the header.
        /// </summary>
        /// <returns>The origin of the diagnostic.</returns>
        public MarkupNode Origin { get; private set; }

        /// <summary>
        /// Gets the single-word category of this diagnostic, for example
        /// <c>error</c> or <c>warning</c>.
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

        /// <summary>
        /// Gets a default rendering of this diagnostic as a bold header
        /// followed by its message body.
        /// </summary>
        public override MarkupNode Fallback
        {
            get
            {
                var titlePart = Text.IsEmpty(Title)
                    ? Title
                    : new Sequence(Title, new Text(": "));

                var header =
                    new Sequence(
                        Origin,
                        new Text(": "),
                        new ColorSpan(
                            new Text(Kind + ": "),
                            ThemeColor),
                        titlePart);

                return new Sequence(
                    new DecorationSpan(header, TextDecoration.Bold),
                    Message);
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            var newOrigin = mapping(Origin);
            var newTitle = mapping(Title);
            var newMessage= mapping(Message);

            if (newOrigin == Origin
                && newTitle == Title
                && newMessage == Message)
            {
                return this;
            }
            else
            {
                return new Diagnostic(
                    newOrigin,
                    Kind,
                    ThemeColor,
                    newTitle,
                    newMessage);
            }
        }
    }
}
