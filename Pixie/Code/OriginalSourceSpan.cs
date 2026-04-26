namespace Pixie.Code
{
    /// <summary>
    /// Represents a span in user-authored original source text.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="SourceSpan"/>, this type is not relative to an arbitrary parsed
    /// source view. It always points at an <see cref="OriginalSourceDocument"/>.
    /// </remarks>
    public struct OriginalSourceSpan
    {
        /// <summary>
        /// Creates an original source span from a document, start offset and length.
        /// </summary>
        /// <param name="document">The original source document.</param>
        /// <param name="start">The zero-based start offset.</param>
        /// <param name="length">The span length.</param>
        public OriginalSourceSpan(
            OriginalSourceDocument document,
            int start,
            int length)
        {
            this.Document = document;
            this.Start = start;
            this.Length = length;
        }

        /// <summary>
        /// Gets the original source document that owns this span.
        /// </summary>
        /// <returns>The original source document.</returns>
        public OriginalSourceDocument Document { get; private set; }

        /// <summary>
        /// Gets the zero-based start offset.
        /// </summary>
        /// <returns>The zero-based start offset.</returns>
        public int Start { get; private set; }

        /// <summary>
        /// Gets the span length.
        /// </summary>
        /// <returns>The span length.</returns>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the exclusive end offset.
        /// </summary>
        /// <returns>The exclusive end offset.</returns>
        public int End => Start + Length;
    }
}
