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
            var titlePart = Text.IsEmpty(diag.Title)
                ? diag.Title
                : new Sequence(diag.Title, new Text(": "));

            var header =
                new Sequence(
                    diag.Origin,
                    new Text(": "),
                    new ColorSpan(
                        new Text(diag.Kind + ": "),
                        diag.ThemeColor),
                    titlePart);

            var bodyState = state
                .WithThemeProperty(
                    HighlightedSourceRenderer.HighlightColorProperty,
                    diag.ThemeColor)
                .WithThemeProperty(
                    RenderThemeProperties.SuppressLeadingSeparatorProperty,
                    true);

            bodyState.Render(new DecorationSpan(header, TextDecoration.Bold));
            if (!Text.IsEmpty(diag.Message))
            {
                bodyState.Terminal.WriteLine();
            }
            bodyState.Render(diag.Message);
        }
    }
}
