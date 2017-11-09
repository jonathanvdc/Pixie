namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A base class for markup node renderers.
    /// </summary>
    public abstract class NodeRenderer
    {
        /// <summary>
        /// Tells if this renderer can render a particular node.
        /// </summary>
        /// <param name="node">A markup node.</param>
        /// <returns>
        /// <c>true</c> if the node can be rendered by this renderer; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanRender(MarkupNode node);

        /// <summary>
        /// Renders a node for a particular render state.
        /// </summary>
        /// <param name="state">The render state.</param>
        /// <returns>
        /// A new render state, to be used by the node's next neighbor, if there is one.
        /// </returns>
        public abstract void Render(MarkupNode node, RenderState state);
    }
}