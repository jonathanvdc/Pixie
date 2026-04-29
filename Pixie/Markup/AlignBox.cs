namespace Pixie.Markup
{
    /// <summary>
    /// An enumeration of possible alignments.
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// Left-aligns content.
        /// </summary>
        Left,
        /// <summary>
        /// Centers content.
        /// </summary>
        Center,
        /// <summary>
        /// Right-aligns content.
        /// </summary>
        Right
    }

    /// <summary>
    /// A block that aligns its contents inside the available width.
    /// </summary>
    public sealed class AlignBox : BlockContainer
    {
        /// <summary>
        /// Creates a left-aligned block.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        public AlignBox(Block contents)
            : this(contents, Alignment.Left)
        { }

        /// <summary>
        /// Creates an aligned block.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        /// <param name="alignment">The alignment to apply.</param>
        public AlignBox(Block contents, Alignment alignment)
            : base(contents)
        {
            this.Alignment = alignment;
        }

        /// <summary>
        /// Gets the alignment applied to the contents.
        /// </summary>
        public Alignment Alignment { get; private set; }

        /// <summary>
        /// Creates a copy of this block container with different contents.
        /// </summary>
        /// <param name="newContents">The new block contents.</param>
        /// <returns>A new aligned block with the updated contents.</returns>
        public override BlockContainer WithContents(Block newContents)
        {
            return new AlignBox(newContents, Alignment);
        }
    }
}
