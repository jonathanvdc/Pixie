using Pixie.Markup;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for sequence nodes.
    /// </summary>
    public sealed class SequenceRenderer : NodeRenderer
    {
        private SequenceRenderer() { }

        /// <summary>
        /// An instance of a sequence node renderer.
        /// </summary>
        public static readonly SequenceRenderer Instance = new SequenceRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is Sequence;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var children = ((Sequence)node).Contents;
            int count = children.Count;
            object suppressVal;
            bool suppressLeading =
                state.ThemeProperties.TryGetValue(
                    RenderThemeProperties.SuppressLeadingSeparatorProperty,
                    out suppressVal)
                && (bool)suppressVal;

            if (suppressLeading && count > 0)
            {
                state.Render(children[0]);
                state = state.WithThemeProperty(
                    RenderThemeProperties.SuppressLeadingSeparatorProperty,
                    false);
                for (int i = 1; i < count; i++)
                {
                    state.Render(children[i]);
                }
                return;
            }

            for (int i = 0; i < count; i++)
            {
                state.Render(children[i]);
            }
        }
    }
}
