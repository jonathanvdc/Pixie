using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A block node that indents its contents once.
    /// </summary>
    public sealed class IndentBox : BlockContainer
    {
        /// <summary>
        /// Creates an indented block.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        public IndentBox(Block contents)
            : base(contents)
        { }

        /// <summary>
        /// Creates an indented block from a sequence of blocks.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        public IndentBox(IReadOnlyList<Block> contents)
            : base(contents)
        { }

        /// <summary>
        /// Creates an indented block from a parameter list of blocks.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        public IndentBox(params Block[] contents)
            : base(contents)
        { }

        /// <summary>
        /// Creates a copy of this block container with different contents.
        /// </summary>
        /// <param name="newContents">The new block contents.</param>
        /// <returns>A new indented block with the updated contents.</returns>
        public override BlockContainer WithContents(Block newContents)
        {
            return new IndentBox(newContents);
        }
    }
}
