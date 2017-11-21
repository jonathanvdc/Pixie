using Pixie.Markup;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A markup node renderer for diagnostic nodes.
    /// </summary>
    public sealed class DiagnosticRenderer : NodeRenderer
    {
        private DiagnosticRenderer() { }

        /// <summary>
        /// An instance of a diagnostic node renderer.
        /// </summary>
        public static readonly DiagnosticRenderer Instance = new DiagnosticRenderer();

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is Diagnostic;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var diag = (Diagnostic)node;
            state
                .WithThemeProperty(
                    HighlightedSourceRenderer.HighlightColorProperty,
                    diag.ThemeColor)
                .Render(diag.Fallback);
        }
    }
}