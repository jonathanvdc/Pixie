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
            state.Terminal.WriteSeparator(2);
            state.Render(para.Contents);
            state.Terminal.WriteSeparator(2);
        }
    }

    /// <summary>
    /// A renderer for box nodes.
    /// </summary>
    public sealed class BoxRenderer : NodeRenderer
    {
        private BoxRenderer() { }

        /// <summary>
        /// An instance of a box node renderer.
        /// </summary>
        public static readonly BoxRenderer Instance = new BoxRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is Box;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var para = (Box)node;
            state.Terminal.WriteSeparator(1);
            state.Render(para.Contents);
            state.Terminal.WriteSeparator(1);
        }
    }
}