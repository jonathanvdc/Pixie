namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that inserts whitespace around its body node.
    /// </summary>
    public sealed class Paragraph : MarkupNode
    {
        /// <summary>
        /// Creates a paragraph from a body node.
        /// </summary>
        /// <param name="Body">A node to insulate in whitespace.</param>
        public Paragraph(MarkupNode Body)
        {
            this.Body = Body;
        }

        /// <summary>
        /// Gets the paragraph's body.
        /// </summary>
        /// <returns>The paragraph's body.</returns>
        public MarkupNode Body { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Body, NewLine.Instance);
    }
}