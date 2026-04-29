using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// Base type for markup that participates in vertical document flow.
    /// </summary>
    public abstract class Block : MarkupElement
    {
        /// <summary>
        /// Attempts to express this block in simpler semantic markup.
        /// </summary>
        public virtual Block Lower()
        {
            return null;
        }

        /// <summary>
        /// Creates a paragraph block from inline text.
        /// </summary>
        public static implicit operator Block(string text)
        {
            return new Paragraph(text);
        }
    }

    /// <summary>
    /// Base type for block nodes that contain a single block child.
    /// </summary>
    public abstract class BlockContainer : Block
    {
        protected BlockContainer(Block contents)
        {
            this.Contents = contents;
        }

        protected BlockContainer(IReadOnlyList<Block> contents)
            : this(new Stack(contents))
        { }

        /// <summary>
        /// Gets the contained block markup.
        /// </summary>
        public Block Contents { get; private set; }

        /// <summary>
        /// Creates a copy of this node with new contents.
        /// </summary>
        public abstract BlockContainer WithContents(Block newContents);
    }
}
