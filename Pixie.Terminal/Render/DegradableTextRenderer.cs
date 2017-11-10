using Pixie.Markup;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A markup node renderer for degradable text nodes.
    /// </summary>
    public sealed class DegradableTextRenderer : NodeRenderer
    {
        private DegradableTextRenderer() { }

        /// <summary>
        /// An instance of a degradable text node renderer.
        /// </summary>
        public static readonly DegradableTextRenderer Instance = new DegradableTextRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is DegradableText;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var degradableText = (DegradableText)node;
            if (state.Terminal.CanRender(degradableText.Contents))
            {
                state.Terminal.Write(degradableText.Contents);
            }
            else
            {
                state.Render(degradableText.Fallback);
            }
        }
    }
}