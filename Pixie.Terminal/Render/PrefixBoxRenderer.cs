using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for prefix-box nodes.
    /// </summary>
    public sealed class PrefixBoxRenderer : NodeRenderer
    {
        private PrefixBoxRenderer() { }

        /// <summary>
        /// An instance of a prefix-box node renderer.
        /// </summary>
        public static readonly PrefixBoxRenderer Instance = new PrefixBoxRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is PrefixBox;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var boxNode = (PrefixBox)node;
            var newState = state;
            var newTerm = LayoutTerminal.Align(
                state.Terminal, Alignment.Left);

            newState = state.WithTerminal(newTerm);
            newTerm.WriteSeparator(1);
            newState.Render(boxNode.Prefix);
            int lineLength = newTerm.BufferedLineLength;
            newTerm.Flush();

            newTerm = LayoutTerminal.AddHorizontalMargin(
                state.Terminal,
                lineLength,
                0);

            newTerm.SuppressPadding();
            newState = newState.WithTerminal(newTerm);
            newState.Render(boxNode.Contents);

            newTerm.WriteSeparator(1);
        }
    }
}