using Pixie.Markup;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A markup node renderer for newline nodes.
    /// </summary>
    public sealed class NewLineRenderer : NodeRenderer
    {
        private NewLineRenderer() { }

        /// <summary>
        /// An instance of a newline renderer.
        /// </summary>
        public static readonly NewLineRenderer Instance = new NewLineRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is NewLine;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            state.Terminal.WriteLine();
        }
    }
}