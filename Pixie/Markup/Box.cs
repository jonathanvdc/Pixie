namespace Pixie.Markup
{
    /// <summary>
    /// An enumeration of line wrapping strategies, which describe how
    /// to turn a single-line string into multiple lines when the
    /// string's contents are too large to render on a single line.
    /// </summary>
    public enum WrappingStrategy
    {
        /// <summary>
        /// Line wrapping is performed on a per-character basis;
        /// words may get spread out across lines.
        /// </summary>
        Character,

        /// <summary>
        /// Line wrapping is performed on a per-word basis.
        /// </summary>
        Word
    }

    /// <summary>
    /// Wraps contents to fit in a box specified by left and right
    /// margins.
    /// </summary>
    public sealed class Box : MarkupNode
    {
        /// <summary>
        /// Creates a box that wraps its contents according to a
        /// particular strategy.
        /// </summary>
        /// <param name="contents">The contents of the box.</param>
        /// <param name="wrapping">The wrapping strategy for the box.</param>
        public Box(
            MarkupNode contents,
            WrappingStrategy wrapping)
            : this(contents, wrapping, 0, 0)
        { }

        /// <summary>
        /// Creates a box that wraps its contents according to the
        /// per-character strategy. A uniform margin is provided.
        /// </summary>
        /// <param name="contents">The contents of the box.</param>
        /// <param name="margin">
        /// A uniform margin that is applied both to the left and the
        /// right of the box.
        /// </param>
        public Box(
            MarkupNode contents,
            int margin)
            : this(contents, margin, margin)
        { }

        /// <summary>
        /// Creates a box that wraps its contents according to a
        /// particular strategy. A uniform margin is provided.
        /// </summary>
        /// <param name="contents">The contents of the box.</param>
        /// <param name="wrapping">The wrapping strategy for the box.</param>
        /// <param name="margin">
        /// A uniform margin that is applied both to the left and the
        /// right of the box.
        /// </param>
        public Box(
            MarkupNode contents,
            WrappingStrategy wrapping,
            int margin)
            : this(contents, wrapping, margin, margin)
        { }

        /// <summary>
        /// Creates a box that wraps its contents according to the
        /// per-character wrapping strategy. A left and right margin
        /// are provided.
        /// </summary>
        /// <param name="contents">The contents of the box.</param>
        /// <param name="leftMargin">A left margin.</param>
        /// <param name="rightMargin">A right margin.</param>
        public Box(
            MarkupNode contents,
            int leftMargin,
            int rightMargin)
            : this(
                contents,
                WrappingStrategy.Character,
                leftMargin,
                rightMargin)
        { }

        /// <summary>
        /// Creates a box that wraps its contents according to a particular
        /// wrapping strategy. A left and right margin are provided.
        /// </summary>
        /// <param name="contents">The contents of the box.</param>
        /// <param name="wrapping">The wrapping strategy for the box.</param>
        /// <param name="leftMargin">A left margin.</param>
        /// <param name="rightMargin">A right margin.</param>
        public Box(
            MarkupNode contents,
            WrappingStrategy wrapping,
            int leftMargin,
            int rightMargin)
        {
            this.Contents = contents;
            this.Wrapping = wrapping;
            this.LeftMargin = leftMargin;
            this.RightMargin = rightMargin;
        }

        /// <summary>
        /// Gets the contents of the box.
        /// </summary>
        /// <returns>The contents node.</returns>
        public MarkupNode Contents { get; private set; }

        /// <summary>
        /// Gets the line wrapping of the box's contents.
        /// </summary>
        /// <returns>The line wrapping strategy.</returns>
        public WrappingStrategy Wrapping { get; private set; }

        /// <summary>
        /// Gets the box's margin on the left.
        /// </summary>
        /// <returns>The box's left margin.</returns>
        public int LeftMargin { get; private set; }

        /// <summary>
        /// Gets the box's margin on the right.
        /// </summary>
        /// <returns>The box's right margin.</returns>
        public int RightMargin { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback => Contents;
    }
}