using System.Collections.Generic;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// Captures the current state of the renderer.
    /// </summary>
    public sealed class RenderState
    {
        /// <summary>
        /// Creates a render state from the given terminal handle.
        /// </summary>
        /// <param name="terminal">A terminal handle.</param>
        public RenderState(TerminalBase terminal)
        {
            this.Terminal = terminal;
            this.parentState = null;
            this.renderers = null;
        }

        /// <summary>
        /// Gets the terminal to render to.
        /// </summary>
        /// <returns>The terminal to render to.</returns>
        public TerminalBase Terminal { get; private set; }

        private RenderState parentState;

        private NodeRenderer[] renderers;

        private bool HasParent => parentState != null;

        /// <summary>
        /// Creates a new render state that includes a sequence of additional
        /// node renderers.
        /// </summary>
        /// <param name="extraRenderers">A sequence of additional renderers.</param>
        /// <returns>A new render state.</returns>
        public RenderState WithRenderers(params NodeRenderer[] extraRenderers)
        {
            return new RenderState(Terminal) { parentState = this, renderers = extraRenderers };
        }

        /// <summary>
        /// Gets the renderer for a markup node.
        /// </summary>
        /// <param name="node">The node to find a renderer for.</param>
        /// <returns>A renderer, if one is found; otherwise, <c>null</c>.</returns>
        public NodeRenderer GetRendererOrNull(MarkupNode node)
        {
            foreach (var item in renderers)
            {
                if (item.CanRender(node))
                    return item;
            }

            if (HasParent)
                return parentState.GetRendererOrNull(node);
            else
                return null;
        }

        /// <summary>
        /// Renders a node.
        /// </summary>
        /// <param name="node">The node to render.</param>
        /// <returns>The render state to use for the node's successor.</returns>
        public RenderState Render(MarkupNode node)
        {
            var renderer = GetRendererOrNull(node);
            if (renderer == null)
            {
                var fallback = node.Fallback;
                if (fallback == null)
                {
                    throw new UnsupportedNodeException(node);
                }
                return Render(fallback);
            }
            else
            {
                return renderer.Render(node, this);
            }
        }
    }
}