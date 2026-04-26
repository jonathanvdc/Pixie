using System.Collections.Generic;

namespace Pixie.Code
{
    /// <summary>
    /// Builds a <see cref="PiecewiseSourceDocument"/> incrementally.
    /// </summary>
    public sealed class PiecewiseSourceDocumentBuilder
    {
        private readonly string identifier;
        private readonly List<SourceDocumentPiece> pieces;

        /// <summary>
        /// Creates a piecewise source document builder.
        /// </summary>
        /// <param name="identifier">The identifier for the assembled document.</param>
        public PiecewiseSourceDocumentBuilder(string identifier)
        {
            this.identifier = identifier;
            this.pieces = new List<SourceDocumentPiece>();
        }

        /// <summary>
        /// Adds a source-backed piece to the assembled document.
        /// </summary>
        /// <param name="origin">The source span to copy into the assembled document.</param>
        /// <returns>This builder.</returns>
        public PiecewiseSourceDocumentBuilder AddSource(SourceSpan origin)
        {
            pieces.Add(SourceDocumentPiece.FromSource(origin));
            return this;
        }

        /// <summary>
        /// Adds generated text with no known source provenance.
        /// </summary>
        /// <param name="text">The generated text.</param>
        /// <returns>This builder.</returns>
        public PiecewiseSourceDocumentBuilder AddText(string text)
        {
            pieces.Add(SourceDocumentPiece.FromText(text));
            return this;
        }

        /// <summary>
        /// Adds generated text anchored to a source span for diagnostics.
        /// </summary>
        /// <param name="text">The generated text.</param>
        /// <param name="anchor">The source span that caused the generated text to exist.</param>
        /// <returns>This builder.</returns>
        public PiecewiseSourceDocumentBuilder AddText(string text, SourceSpan anchor)
        {
            pieces.Add(SourceDocumentPiece.FromText(text, anchor));
            return this;
        }

        /// <summary>
        /// Creates a piecewise source document from the pieces added so far.
        /// </summary>
        /// <returns>A piecewise source document.</returns>
        public PiecewiseSourceDocument Build()
        {
            return new PiecewiseSourceDocument(identifier, pieces);
        }
    }
}
