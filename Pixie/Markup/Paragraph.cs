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

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new Paragraph(newContents);
        }
    }

    /// <summary>
    /// A markup node that visually separates its body from
    /// other markup nodes by putting its body on different
    /// lines from the box' successors and predecessors.
    /// </summary>
    public sealed class Box : ContainerNode
    {
        /// <summary>
        /// Creates a box from a body node.
        /// </summary>
        /// <param name="contents">A node to insulate.</param>
        public Box(MarkupNode contents)
            : base(contents)
        { }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Contents, NewLine.Instance);

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new Box(newContents);
        }
    }
}