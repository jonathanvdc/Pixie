namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that inserts whitespace around its body.
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
        /// Gets the paragraph's body.
        /// </summary>
        /// <returns>The paragraph's body.</returns>
        public MarkupNode Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Contents, NewLine.Instance);
    }
}