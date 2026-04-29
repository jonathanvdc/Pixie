using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A vertical sequence of block markup nodes.
    /// </summary>
    public sealed class Stack : Block
    {
        public Stack(params Block[] contents)
            : this((IReadOnlyList<Block>)contents)
        { }

        public Stack(IReadOnlyList<Block> contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the stack's block children.
        /// </summary>
        public IReadOnlyList<Block> Contents { get; private set; }
    }
}
