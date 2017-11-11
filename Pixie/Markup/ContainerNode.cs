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
    }
}