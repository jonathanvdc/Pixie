using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for indentation box nodes.
    /// </summary>
    public sealed class IndentBoxRenderer : NodeRenderer
    {
        private IndentBoxRenderer() { }

        /// <summary>
        /// An instance of a indentation node renderer.
        /// </summary>
        public static readonly IndentBoxRenderer Instance = new IndentBoxRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is IndentBox;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var boxNode = (IndentBox)node;
            var newState = state;
            var newTerm = LayoutTerminal.AddHorizontalMargin(
                state.Terminal,
                4,
                0);

            newState = newTerm.StartLayoutBox(state);
            newState.Render(boxNode.Contents);
            newTerm.EndLayoutBox();
        }
    }
}
