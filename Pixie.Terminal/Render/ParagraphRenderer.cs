using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for paragraph nodes.
    /// </summary>
    public sealed class ParagraphRenderer : NodeRenderer
    {
        private ParagraphRenderer() { }

        /// <summary>
        /// An instance of a paragraph node renderer.
        /// </summary>
        public static readonly ParagraphRenderer Instance = new ParagraphRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is Paragraph;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var para = (Paragraph)node;
            var newState = state;
            if (para.Alignment != Alignment.Left)
            {
                var alignedTerm = new AlignedTerminal(para.Alignment, state.Terminal);
                newState = state.WithTerminal(alignedTerm);
            }

            newState.Terminal.WriteSeparator(2);
            newState.Render(para.Body);
            newState.Terminal.WriteSeparator(2);
        }
    }
}