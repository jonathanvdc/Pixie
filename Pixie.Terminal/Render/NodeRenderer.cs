namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A base class for markup node renderers.
    /// </summary>
    public abstract class NodeRenderer
    {
        /// <summary>
        /// Renders a node for a particular render state.
        /// </summary>
        /// <param name="state">The render state.</param>
        /// <returns>
        /// A new render state, to be used by the node's next neighbor, if there is one.
        /// </returns>
        public abstract RenderState Render(MarkupNode node, RenderState state);
    }
}