using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// Describes how a terminal renderer may wrap inline content.
    /// </summary>
    public enum WrappingStrategy
    {
        /// <summary>
        /// Allows wrapping at any character boundary.
        /// </summary>
        Character,
        /// <summary>
        /// Wraps at word boundaries where possible.
        /// </summary>
        Word
    }

    /// <summary>
    /// A block that applies wrapping and horizontal margins to its contents.
    /// </summary>
    public sealed class WrapBox : BlockContainer
    {
        /// <summary>
        /// Creates a wrapped block with no horizontal margins.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        /// <param name="wrapping">The wrapping strategy to apply.</param>
        public WrapBox(Block contents, WrappingStrategy wrapping)
            : this(contents, wrapping, 0, 0)
        { }

        /// <summary>
        /// Creates a character-wrapped block with equal horizontal margins.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        /// <param name="margin">The left and right margin width.</param>
        public WrapBox(Block contents, int margin)
            : this(contents, WrappingStrategy.Character, margin, margin)
        { }

        /// <summary>
        /// Creates a wrapped block with equal horizontal margins.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        /// <param name="wrapping">The wrapping strategy to apply.</param>
        /// <param name="margin">The left and right margin width.</param>
        public WrapBox(Block contents, WrappingStrategy wrapping, int margin)
            : this(contents, wrapping, margin, margin)
        { }

        /// <summary>
        /// Creates a character-wrapped block with horizontal margins.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        /// <param name="leftMargin">The left margin width.</param>
        /// <param name="rightMargin">The right margin width.</param>
        public WrapBox(Block contents, int leftMargin, int rightMargin)
            : this(contents, WrappingStrategy.Character, leftMargin, rightMargin)
        { }

        /// <summary>
        /// Creates a wrapped block with horizontal margins.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        /// <param name="wrapping">The wrapping strategy to apply.</param>
        /// <param name="leftMargin">The left margin width.</param>
        /// <param name="rightMargin">The right margin width.</param>
        public WrapBox(Block contents, WrappingStrategy wrapping, int leftMargin, int rightMargin)
            : base(contents)
        {
            this.Wrapping = wrapping;
            this.LeftMargin = leftMargin;
            this.RightMargin = rightMargin;
        }

        /// <summary>
        /// Gets the wrapping strategy applied to the contents.
        /// </summary>
        public WrappingStrategy Wrapping { get; private set; }

        /// <summary>
        /// Gets the left margin width.
        /// </summary>
        public int LeftMargin { get; private set; }

        /// <summary>
        /// Gets the right margin width.
        /// </summary>
        public int RightMargin { get; private set; }

        /// <summary>
        /// Creates a copy of this block container with different contents.
        /// </summary>
        /// <param name="newContents">The new block contents.</param>
        /// <returns>A new wrapped block with the updated contents.</returns>
        public override BlockContainer WithContents(Block newContents)
        {
            return new WrapBox(newContents, Wrapping, LeftMargin, RightMargin);
        }

        /// <summary>
        /// Creates a word-wrapped block with a standard left indent.
        /// </summary>
        /// <param name="node">The block to wrap.</param>
        /// <returns>A word-wrapped block with a standard left indent.</returns>
        public static WrapBox IndentAndWordWrap(Block node)
        {
            return new WrapBox(node, WrappingStrategy.Word, 4, 0);
        }

        /// <summary>
        /// Creates a word-wrapped stack with a standard left indent.
        /// </summary>
        /// <param name="nodes">The blocks to wrap.</param>
        /// <returns>A word-wrapped block with a standard left indent.</returns>
        public static WrapBox IndentAndWordWrap(IReadOnlyList<Block> nodes)
        {
            return IndentAndWordWrap(new Stack(nodes));
        }

        /// <summary>
        /// Creates a word-wrapped stack from a parameter list of blocks with a standard left indent.
        /// </summary>
        /// <param name="nodes">The blocks to wrap.</param>
        /// <returns>A word-wrapped block with a standard left indent.</returns>
        public static WrapBox IndentAndWordWrap(params Block[] nodes)
        {
            return IndentAndWordWrap(new Stack(nodes));
        }

        /// <summary>
        /// Creates a word-wrapped block.
        /// </summary>
        /// <param name="node">The block to wrap.</param>
        /// <returns>A word-wrapped block.</returns>
        public static WrapBox WordWrap(Block node)
        {
            return new WrapBox(node, WrappingStrategy.Word);
        }

        /// <summary>
        /// Creates a word-wrapped stack.
        /// </summary>
        /// <param name="nodes">The blocks to wrap.</param>
        /// <returns>A word-wrapped block.</returns>
        public static WrapBox WordWrap(IReadOnlyList<Block> nodes)
        {
            return WordWrap(new Stack(nodes));
        }

        /// <summary>
        /// Creates a word-wrapped stack from a parameter list of blocks.
        /// </summary>
        /// <param name="nodes">The blocks to wrap.</param>
        /// <returns>A word-wrapped block.</returns>
        public static WrapBox WordWrap(params Block[] nodes)
        {
            return WordWrap(new Stack(nodes));
        }
    }
}
