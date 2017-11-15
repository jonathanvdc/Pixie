using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for decoration spans.
    /// </summary>
    public sealed class DecorationSpanRenderer : NodeRenderer
    {
        private DecorationSpanRenderer() { }

        /// <summary>
        /// An instance of a decoration span renderer.
        /// </summary>
        public static readonly DecorationSpanRenderer Instance = new DecorationSpanRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is DecorationSpan;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var decorationNode = (DecorationSpan)node;

            state.Terminal.Style.PushDecoration(
                decorationNode.Decoration,
                decorationNode.UpdateDecoration);
            try
            {
                state.Render(decorationNode.Contents);
            }
            finally
            {
                state.Terminal.Style.PopStyle();
            }
        }
    }
}
