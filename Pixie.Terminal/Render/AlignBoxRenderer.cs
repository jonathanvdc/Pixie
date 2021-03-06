using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for align-box nodes.
    /// </summary>
    public sealed class AlignBoxRenderer : NodeRenderer
    {
        private AlignBoxRenderer() { }

        /// <summary>
        /// An instance of an align-box node renderer.
        /// </summary>
        public static readonly AlignBoxRenderer Instance = new AlignBoxRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is AlignBox;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var boxNode = (AlignBox)node;
            var newState = state;
            var newTerm = LayoutTerminal.Align(
                state.Terminal, boxNode.Alignment);

            newState = newTerm.StartLayoutBox(state);
            newState.Render(boxNode.Contents);
            newTerm.EndLayoutBox();
        }
    }
}