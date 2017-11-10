using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
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
            var boxNode = (Box)node;
            var newState = state;
            var newTerm = AlignedTerminal.Wrap(
                AlignedTerminal.AddHorizontalMargin(
                    state.Terminal,
                    boxNode.LeftMargin,
                    boxNode.RightMargin),
                boxNode.Wrapping);

            newState = state.WithTerminal(newTerm);
            newTerm.WriteSeparator(1);
            newState.Render(boxNode.Contents);
            newTerm.WriteSeparator(1);
        }
    }
}