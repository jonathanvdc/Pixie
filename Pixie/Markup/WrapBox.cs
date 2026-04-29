using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// Describes how a terminal renderer may wrap inline content.
    /// </summary>
    public enum WrappingStrategy
    {
        Character,
        Word
    }

    /// <summary>
    /// A block that applies wrapping and horizontal margins to its contents.
    /// </summary>
    public sealed class WrapBox : BlockContainer
    {
        public WrapBox(Block contents, WrappingStrategy wrapping)
            : this(contents, wrapping, 0, 0)
        { }

        public WrapBox(Block contents, int margin)
            : this(contents, WrappingStrategy.Character, margin, margin)
        { }

        public WrapBox(Block contents, WrappingStrategy wrapping, int margin)
            : this(contents, wrapping, margin, margin)
        { }

        public WrapBox(Block contents, int leftMargin, int rightMargin)
            : this(contents, WrappingStrategy.Character, leftMargin, rightMargin)
        { }

        public WrapBox(Block contents, WrappingStrategy wrapping, int leftMargin, int rightMargin)
            : base(contents)
        {
            this.Wrapping = wrapping;
            this.LeftMargin = leftMargin;
            this.RightMargin = rightMargin;
        }

        public WrappingStrategy Wrapping { get; private set; }

        public int LeftMargin { get; private set; }

        public int RightMargin { get; private set; }

        public override BlockContainer WithContents(Block newContents)
        {
            return new WrapBox(newContents, Wrapping, LeftMargin, RightMargin);
        }

        public static WrapBox IndentAndWordWrap(Block node)
        {
            return new WrapBox(node, WrappingStrategy.Word, 4, 0);
        }

        public static WrapBox IndentAndWordWrap(IReadOnlyList<Block> nodes)
        {
            return IndentAndWordWrap(new Stack(nodes));
        }

        public static WrapBox IndentAndWordWrap(params Block[] nodes)
        {
            return IndentAndWordWrap(new Stack(nodes));
        }

        public static WrapBox WordWrap(Block node)
        {
            return new WrapBox(node, WrappingStrategy.Word);
        }

        public static WrapBox WordWrap(IReadOnlyList<Block> nodes)
        {
            return WordWrap(new Stack(nodes));
        }

        public static WrapBox WordWrap(params Block[] nodes)
        {
            return WordWrap(new Stack(nodes));
        }
    }
}
