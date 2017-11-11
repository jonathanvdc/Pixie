namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that inserts whitespace around its body.
    /// </summary>
    public sealed class Paragraph : ContainerNode
    {
        /// <summary>
        /// Creates a paragraph from a body node.
        /// </summary>
        /// <param name="contents">A node to insulate in whitespace.</param>
        public Paragraph(MarkupNode contents)
            : base(contents)
        { }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Contents, NewLine.Instance);
    }
}