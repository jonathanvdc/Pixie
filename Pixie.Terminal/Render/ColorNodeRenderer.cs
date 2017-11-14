using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for color nodes.
    /// </summary>
    public sealed class ColorNodeRenderer : NodeRenderer
    {
        private ColorNodeRenderer() { }

        /// <summary>
        /// An instance of a color node renderer.
        /// </summary>
        public static readonly ColorNodeRenderer Instance = new ColorNodeRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is ColorNode;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var colorNode = (ColorNode)node;

            state.Terminal.Style.PushForegroundColor(colorNode.ForegroundColor);
            state.Terminal.Style.PushBackgroundColor(colorNode.BackgroundColor);
            try
            {
                state.Render(colorNode.Contents);
            }
            finally
            {
                state.Terminal.Style.PopStyle();
                state.Terminal.Style.PopStyle();
            }
        }
    }
}