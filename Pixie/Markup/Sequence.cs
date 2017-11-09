using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that consists of a sequence of other nodes.
    /// </summary>
    public sealed class Sequence : MarkupNode
    {
        /// <summary>
        /// Creates a sequence node from a list of nodes.
        /// </summary>
        /// <param name="contents">A list of nodes to render in sequence.</param>
        public Sequence(params MarkupNode[] contents)
            : this((IReadOnlyList<MarkupNode>)contents)
        { }

        /// <summary>
        /// Creates a sequence node from a list of nodes.
        /// </summary>
        /// <param name="contents">A list of nodes to render in sequence.</param>
        public Sequence(IReadOnlyList<MarkupNode> contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the list of markup nodes that make up this node's contents.
        /// </summary>
        /// <returns>The content markup nodes.</returns>
        public IReadOnlyList<MarkupNode> Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback => null;
    }
}