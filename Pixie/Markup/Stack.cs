using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A vertical sequence of block markup nodes.
    /// </summary>
    public sealed class Stack : Block
    {
        /// <summary>
        /// Creates a vertical stack from a parameter list of block nodes.
        /// </summary>
        /// <param name="contents">The stacked block contents.</param>
        public Stack(params Block[] contents)
            : this((IReadOnlyList<Block>)contents)
        { }

        /// <summary>
        /// Creates a vertical stack of block nodes.
        /// </summary>
        /// <param name="contents">The stacked block contents.</param>
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
