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
            this.themeProps = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a render state from a parent state and a dictionary
        /// of theme properties.
        /// </summary>
        /// <param name="parent">A parent state.</param>
        /// <param name="themeProps">A dictionary of theme properties.</param>
        private RenderState(RenderState parent, Dictionary<string, object> themeProps)
        {
            this.Terminal = parent.Terminal;
            this.parentState = parent;
            this.renderers = null;
            this.themeProps = themeProps;
        }

        /// <summary>
        /// Gets the terminal to render to.
        /// </summary>
        /// <returns>The terminal to render to.</returns>
        public TerminalBase Terminal { get; private set; }

        private RenderState parentState;

        private NodeRenderer[] renderers;

        private Dictionary<string, object> themeProps;

        private bool HasParent => parentState != null;

        /// <summary>
        /// Gets a mapping of theme resource strings to values.
        /// </summary>
        public IReadOnlyDictionary<string, object> ThemeProperties => themeProps;

        /// <summary>
        /// Creates a new render state that includes a sequence of additional
        /// node renderers.
        /// </summary>
        /// <param name="extraRenderers">A sequence of additional renderers.</param>
        /// <returns>A new render state.</returns>
        public RenderState WithRenderers(params NodeRenderer[] extraRenderers)
        {
            return new RenderState(this, themeProps)
            {
                renderers = extraRenderers
            };
        }

        /// <summary>
        /// Creates a new render state that inherits all information from this
        /// state, except for the terminal, which it replaces.
        /// </summary>
        /// <param name="newTerminal">The terminal to use in the new state.</param>
        /// <returns>A new render state.</returns>
        public RenderState WithTerminal(TerminalBase newTerminal)
        {
            return new RenderState(this, themeProps)
            {
                Terminal = newTerminal
            };
        }

        /// <summary>
        /// Creates a new render state that inherits all information from this
        /// state, except for a theme property, which it sets.
        /// </summary>
        /// <param name="property">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A new render state.</returns>
        public RenderState WithThemeProperty(string property, object value)
        {
            var props = new Dictionary<string, object>(themeProps);
            props[property] = value;
            return new RenderState(parentState, props);
        }

        /// <summary>
        /// Gets the renderer for a markup node.
        /// </summary>
        /// <param name="node">The node to find a renderer for.</param>
        /// <returns>A renderer, if one is found; otherwise, <c>null</c>.</returns>
        public NodeRenderer GetRendererOrNull(MarkupNode node)
        {
            if (renderers != null)
            {
                foreach (var item in renderers)
                {
                    if (item.CanRender(node))
                    {
                        return item;
                    }
                }
            }

            if (HasParent)
            {
                return parentState.GetRendererOrNull(node);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Renders a node.
        /// </summary>
        /// <param name="node">The node to render.</param>
        /// <returns>The render state to use for the node's successor.</returns>
        public void Render(MarkupNode node)
        {
            var renderer = GetRendererOrNull(node);
            if (renderer == null)
            {
                var fallback = node.Fallback;
                if (fallback == null)
                {
                    throw new UnsupportedNodeException(node);
                }
                Render(fallback);
            }
            else
            {
                renderer.Render(node, this);
            }
        }
    }
}