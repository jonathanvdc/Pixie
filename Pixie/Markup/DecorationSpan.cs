using System;

namespace Pixie.Markup
{
    /// <summary>
    /// Describes an enumeration of text decorations.
    /// </summary>
    [FlagsAttribute]
    public enum TextDecoration
    {
        /// <summary>
        /// Text is left undecorated.
        /// </summary>
        None = 0,

        /// <summary>
        /// Text is rendered in bold.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Text is rendered in italics.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Text is underlined.
        /// </summary>
        Underline = 4,

        /// <summary>
        /// Text is struck through.
        /// </summary>
        Strikethrough = 8
    }

    /// <summary>
    /// A markup node that toggles text decorations for its contents.
    /// </summary>
    public sealed class DecorationSpan : ContainerNode
    {
        /// <summary>
        /// Creates a decoration span from the given contents and decoration.
        /// </summary>
        /// <param name="contents">The decoration span's contents.</param>
        /// <param name="decoration">The decoration to apply to the contents.</param>
        public DecorationSpan(
            MarkupNode contents,
            TextDecoration decoration)
            : this(contents, decoration, UnifyDecorations)
        { }

        /// <summary>
        /// Creates a decoration span from the given contents and decoration.
        /// </summary>
        /// <param name="contents">The decoration span's contents.</param>
        /// <param name="decoration">The decoration to apply to the contents.</param>
        /// <param name="updateDecoration">An operator that updates the decorations.</param>
        public DecorationSpan(
            MarkupNode contents,
            TextDecoration decoration,
            Func<TextDecoration, TextDecoration, TextDecoration> updateDecoration)
            : base(contents)
        {
            this.Decoration = decoration;
            this.UpdateDecoration = updateDecoration;
        }

        /// <summary>
        /// Gets the decoration to apply. Text decorations are encoded
        /// as a bit field, so this field may specify zero or more
        /// decorations.
        /// </summary>
        /// <returns>The decoration to apply.</returns>
        public TextDecoration Decoration { get; private set; }

        /// <summary>
        /// Gets the binary operator that is used to update the text
        /// decoration of this span's contents. The first argument to
        /// the operator represents the previous text decoration,
        /// the second argument represents the text decoration
        /// supplied by a span and the return value are the final
        /// text decorations for this span's contents.
        /// </summary>
        /// <returns>A binary operator.</returns>
        public Func<TextDecoration, TextDecoration, TextDecoration> UpdateDecoration { get; private set; }

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new DecorationSpan(newContents, Decoration, UpdateDecoration);
        }

        /// <inheritdoc/>
        public override MarkupNode Fallback => Contents;

        /// <summary>
        /// Returns the second text decoration.
        /// </summary>
        /// <param name="first">The first text decoration.</param>
        /// <param name="second">The second text decoration.</param>
        /// <returns>The second text decoration.</returns>
        public static TextDecoration ReplaceDecorations(
            TextDecoration first,
            TextDecoration second)
        {
            return second;
        }

        /// <summary>
        /// Computes the union of two text decorations.
        /// </summary>
        /// <param name="first">The first text decoration.</param>
        /// <param name="second">The second text decoration.</param>
        /// <returns>The union of the text decorations.</returns>
        public static TextDecoration UnifyDecorations(
            TextDecoration first,
            TextDecoration second)
        {
            return first | second;
        }

        /// <summary>
        /// Computes the intersection of two text decorations.
        /// </summary>
        /// <param name="first">The first text decoration.</param>
        /// <param name="second">The second text decoration.</param>
        /// <returns>The intersection of the text decorations.</returns>
        public static TextDecoration IntersectDecorations(
            TextDecoration first,
            TextDecoration second)
        {
            return first & second;
        }

        /// <summary>
        /// Computes the exclusive or of two text decorations.
        /// </summary>
        /// <param name="first">The first text decoration.</param>
        /// <param name="second">The second text decoration.</param>
        /// <returns>The exclusive or of the text decorations.</returns>
        public static TextDecoration ToggleDecorations(
            TextDecoration first,
            TextDecoration second)
        {
            return first ^ second;
        }
    }
}