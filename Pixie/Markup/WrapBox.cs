using System;
using System.Collections.Generic;

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
    public sealed class WrapBox : ContainerNode
    {
        /// <summary>
        /// Creates a box that wraps its contents according to a
        /// particular strategy.
        /// </summary>
        /// <param name="contents">The contents of the box.</param>
        /// <param name="wrapping">The wrapping strategy for the box.</param>
        public WrapBox(
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
        public WrapBox(
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
        public WrapBox(
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
        public WrapBox(
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
        public WrapBox(
            MarkupNode contents,
            WrappingStrategy wrapping,
            int leftMargin,
            int rightMargin)
            : base(contents)
        {
            this.Wrapping = wrapping;
            this.LeftMargin = leftMargin;
            this.RightMargin = rightMargin;
        }

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
        public override MarkupNode Fallback =>
            new Sequence(NewLine.Instance, Contents, NewLine.Instance);

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new WrapBox(newContents, Wrapping, LeftMargin, RightMargin);
        }

        /// <summary>
        /// Creates a wrap box that indents and word-wraps a node.
        /// </summary>
        /// <param name="node">The node to indent and word-wrap.</param>
        /// <returns>An indented, word-wrapped node.</returns>
        public static WrapBox IndentAndWordWrap(MarkupNode node)
        {
            return new WrapBox(node, WrappingStrategy.Word, 4, 0);
        }

        /// <summary>
        /// Creates a wrap box that indents and word-wraps a sequence of nodes.
        /// </summary>
        /// <param name="nodes">The nodes to indent and word-wrap.</param>
        /// <returns>An indented, word-wrapped node.</returns>
        public static WrapBox IndentAndWordWrap(IReadOnlyList<MarkupNode> nodes)
        {
            return IndentAndWordWrap(new Sequence(nodes));
        }

        /// <summary>
        /// Creates a wrap box that indents and word-wraps a sequence of nodes.
        /// </summary>
        /// <param name="nodes">The nodes to indent and word-wrap.</param>
        /// <returns>An indented, word-wrapped node.</returns>
        public static WrapBox IndentAndWordWrap(params MarkupNode[] nodes)
        {
            return IndentAndWordWrap(new Sequence(nodes));
        }

        /// <summary>
        /// Creates a wrap box that word-wraps a node.
        /// </summary>
        /// <param name="node">The node to word-wrap.</param>
        /// <returns>A word-wrapped node.</returns>
        public static WrapBox WordWrap(MarkupNode node)
        {
            return new WrapBox(node, WrappingStrategy.Word);
        }

        /// <summary>
        /// Creates a wrap box that word-wraps a sequence of nodes.
        /// </summary>
        /// <param name="nodes">The nodes to word-wrap.</param>
        /// <returns>A word-wrapped node.</returns>
        public static WrapBox WordWrap(IReadOnlyList<MarkupNode> nodes)
        {
            return WordWrap(new Sequence(nodes));
        }

        /// <summary>
        /// Creates a wrap box that word-wraps a sequence of nodes.
        /// </summary>
        /// <param name="nodes">The nodes to word-wrap.</param>
        /// <returns>A word-wrapped node.</returns>
        public static WrapBox WordWrap(params MarkupNode[] nodes)
        {
            return WordWrap(new Sequence(nodes));
        }
    }
}