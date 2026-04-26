namespace Pixie.Code
{
    /// <summary>
    /// Represents one display line in an original source document.
    /// </summary>
    /// <remarks>
    /// Source lines belong to <see cref="OriginalSourceDocument"/> because only
    /// original documents have a user-facing line grid. Derived documents expose
    /// offsets in their assembled text and resolve those offsets back to original
    /// source for diagnostic display.
    /// </remarks>
    public readonly struct SourceLine
    {
        /// <summary>
        /// Creates a source line.
        /// </summary>
        /// <param name="document">The original source document that owns the line.</param>
        /// <param name="index">The zero-based line index.</param>
        /// <param name="start">The zero-based offset of the first character in the line.</param>
        /// <param name="length">The line length, excluding line terminators.</param>
        public SourceLine(
            OriginalSourceDocument document,
            int index,
            int start,
            int length)
        {
            this.Document = document;
            this.Index = index;
            this.Start = start;
            this.Length = length;
        }

        /// <summary>
        /// Gets the original source document that owns this line.
        /// </summary>
        /// <returns>The original source document.</returns>
        public OriginalSourceDocument Document { get; }

        /// <summary>
        /// Gets the zero-based line index.
        /// </summary>
        /// <returns>The zero-based line index.</returns>
        public int Index { get; }

        /// <summary>
        /// Gets the zero-based offset of the first character in the line.
        /// </summary>
        /// <returns>The zero-based start offset.</returns>
        public int Start { get; }

        /// <summary>
        /// Gets the line length, excluding line terminators.
        /// </summary>
        /// <returns>The line length.</returns>
        public int Length { get; }

        /// <summary>
        /// Gets the exclusive end offset of the line, excluding line terminators.
        /// </summary>
        /// <returns>The exclusive end offset.</returns>
        public int End => Start + Length;

        /// <summary>
        /// Gets the line text, excluding line terminators.
        /// </summary>
        /// <returns>The line text.</returns>
        public string Text => Document.GetText(Start, Length);
    }
}
