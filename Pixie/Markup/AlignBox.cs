namespace Pixie.Markup
{
    /// <summary>
    /// An enumeration of possible alignments.
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// Align contents to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Center contents.
        /// </summary>
        Center,

        /// <summary>
        /// Align contents to the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// A markup node that aligns its contents.
    /// </summary>
    public sealed class AlignBox : ContainerNode
    {
        /// <summary>
        /// Creates an align-box from a body node.
        /// </summary>
        /// <param name="contents">A node to insulate in whitespace.</param>
        public AlignBox(MarkupNode contents)
            : this(contents, Alignment.Left)
        { }

        /// <summary>
        /// Creates an align-box from a body node and an alignment.
        /// </summary>
        /// <param name="contents">A node to insulate in whitespace.</param>
        /// <param name="alignment">The alignment for the align-box.</param>
        public AlignBox(MarkupNode contents, Alignment alignment)
            : base(contents)
        {
            this.Alignment = alignment;
        }

        /// <summary>
        /// Gets the align-box's alignment.
        /// </summary>
        /// <returns>The align-box's alignment.</returns>
        public Alignment Alignment { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Contents, NewLine.Instance);

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new AlignBox(newContents, Alignment);
        }
    }
}