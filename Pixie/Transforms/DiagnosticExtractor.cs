using System;

namespace Pixie.Transforms
{
    /// <summary>
    /// A transformation that turns log entries into diagnostics.
    /// </summary>
    public sealed class DiagnosticExtractor
    {
        /// <summary>
        /// Creates a diagnostic extractor.
        /// </summary>
        /// <param name="defaultOrigin">
        /// The default origin to use, for when a log entry
        /// does not specify an origin.
        /// </param>
        /// <param name="diagnosticKind">
        /// The kind of diagnostic to provide.
        /// </param>
        /// <param name="diagnosticThemeColor">
        /// 
        /// </param>
        public DiagnosticExtractor(
            MarkupNode defaultOrigin,
            string diagnosticKind,
            Color diagnosticThemeColor)
        {
            this.DefaultOrigin = defaultOrigin;
            this.DiagnosticKind = diagnosticKind;
            this.DiagnosticThemeColor = diagnosticThemeColor;
        }

        /// <summary>
        /// Gets the default origin to use, for when a log entry
        /// does not specify an origin.
        /// </summary>
        /// <returns>The default origin.</returns>
        public MarkupNode DefaultOrigin { get; private set; }

        /// <summary>
        /// Gets the kind of diagnostic to provide.
        /// </summary>
        /// <returns>The kind of diagnostic.</returns>
        public string DiagnosticKind { get; private set; }

        /// <summary>
        /// Gets the theme color for diagnostics.
        /// </summary>
        /// <returns>The theme color for diagnostics.</returns>
        public Color DiagnosticThemeColor { get; private set; }
    }

    /// <summary>
    /// A node visitor that helps turn log entries into diagnostics.
    /// </summary>
    internal sealed class DiagnosticExtractingVisitor : MarkupVisitor
    {
        protected override bool IsOfInterest(MarkupNode node)
        {
            throw new NotImplementedException();
        }

        protected override MarkupNode VisitInteresting(MarkupNode node)
        {
            throw new NotImplementedException();
        }
    }
}