using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for color spans.
    /// </summary>
    public sealed class ColorSpanRenderer : NodeRenderer
    {
        private ColorSpanRenderer() { }

        /// <summary>
        /// An instance of a color span renderer.
        /// </summary>
        public static readonly ColorSpanRenderer Instance = new ColorSpanRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is ColorSpan;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var colorNode = (ColorSpan)node;

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