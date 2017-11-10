namespace Pixie.Markup
{
    public enum Alignment
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// A markup node that inserts whitespace around its body node.
    /// </summary>
    public sealed class Paragraph : MarkupNode
    {
        /// <summary>
        /// Creates a paragraph from a body node.
        /// </summary>
        /// <param name="contents">A node to insulate in whitespace.</param>
        public Paragraph(MarkupNode contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Creates a paragraph from a body node and an alignment.
        /// </summary>
        /// <param name="contents">A node to insulate in whitespace.</param>
        /// <param name="Alignment">The alignment for the paragraph.</param>
        public Paragraph(MarkupNode contents, Alignment alignment)
        {
            this.Contents = contents;
            this.Alignment = alignment;
        }

        /// <summary>
        /// Gets the paragraph's body.
        /// </summary>
        /// <returns>The paragraph's body.</returns>
        public MarkupNode Contents { get; private set; }

        /// <summary>
        /// Gets the paragraph's alignment.
        /// </summary>
        /// <returns>The paragraph's alignment.</returns>
        public Alignment Alignment { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Contents, NewLine.Instance);
    }
}