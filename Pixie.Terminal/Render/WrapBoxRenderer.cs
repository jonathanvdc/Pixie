using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for box nodes.
    /// </summary>
    public sealed class WrapBoxRenderer : NodeRenderer
    {
        private WrapBoxRenderer() { }

        /// <summary>
        /// An instance of a box node renderer.
        /// </summary>
        public static readonly WrapBoxRenderer Instance = new WrapBoxRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is WrapBox;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var boxNode = (WrapBox)node;
            var newState = state;
            var newTerm = LayoutTerminal.Wrap(
                LayoutTerminal.AddHorizontalMargin(
                    state.Terminal,
                    boxNode.LeftMargin,
                    boxNode.RightMargin),
                boxNode.Wrapping);

            newState = newTerm.StartLayoutBox(state);
            newState.Render(boxNode.Contents);
            newTerm.EndLayoutBox();
        }
    }
}