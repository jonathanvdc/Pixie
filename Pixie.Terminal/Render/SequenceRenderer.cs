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
            for (int i = 0; i < count; i++)
            {
                state.Render(children[i]);
            }
        }
    }
}