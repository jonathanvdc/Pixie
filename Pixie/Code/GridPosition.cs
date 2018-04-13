namespace Pixie.Code
{
    /// <summary>
    /// Specifies a position in a source document as a line index
    /// and an offset.
    /// </summary>
    public struct GridPosition
    {
        /// <summary>
        /// Creates a grid position from a line index and offset.
        /// </summary>
        /// <param name="lineIndex">A line index.</param>
        /// <param name="offset">An offset in the line.</param>
        public GridPosition(int lineIndex, int offset)
        {
            this.LineIndex = lineIndex;
            this.Offset = offset;
        }

        /// <summary>
        /// Gets the zero-based line index of this grid position.
        /// </summary>
        /// <returns>The line index.</returns>
        public int LineIndex { get; private set; }

        /// <summary>
        /// Gets the zero-based offset in the line selected
        /// by this grid position.
        /// </summary>
        /// <returns>The offset.</returns>
        public int Offset { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "(" + LineIndex + ", " + Offset + ")";
        }
    }
}