using Pixie.Markup;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A markup node renderer for text nodes.
    /// </summary>
    public sealed class TextRenderer : NodeRenderer
    {
        private TextRenderer() { }

        /// <summary>
        /// An instance of a text node renderer.
        /// </summary>
        public static readonly TextRenderer Instance = new TextRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is TextNode;
        }

        /// <inheritdoc/>
        public override RenderState Render(MarkupNode node, RenderState state)
        {
            state.Terminal.Write(((TextNode)node).Text);
            return state;
        }
    }
}