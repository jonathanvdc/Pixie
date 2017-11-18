using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A base class for markup nodes that act as a container for exactly
    /// one other node.
    /// </summary>
    public abstract class ContainerNode : MarkupNode
    {
        /// <summary>
        /// Creates a container from a body node.
        /// </summary>
        /// <param name="contents">The contents of the container.</param>
        public ContainerNode(MarkupNode contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the container's body.
        /// </summary>
        /// <returns>The container's body.</returns>
        public MarkupNode Contents { get; private set; }

        /// <summary>
        /// Creates a copy of this container node that has a particular
        /// contents node.
        /// </summary>
        /// <param name="newContents">The contents of the container.</param>
        /// <returns>A container node with the given contents.</returns>
        public abstract ContainerNode WithContents(MarkupNode newContents);

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            var newContents = mapping(Contents);
            if (Contents == newContents)
                return this;
            else
                return WithContents(newContents);
        }
    }
}