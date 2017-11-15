using System;

namespace Pixie.Code
{
    /// <summary>
    /// Specifies a span of source code.
    /// </summary>
    public struct SourceSpan
    {
        /// <summary>
        /// Creates a source span from a document, an offset and
        /// a span length.
        /// </summary>
        /// <param name="document">
        /// The source document this span is a part of.
        /// </param>
        /// <param name="offset">
        /// The offset of the first character in the span.
        /// </param>
        /// <param name="length">
        /// The number of characters in the span.
        /// </param>
        public SourceSpan(
            SourceDocument document,
            int offset,
            int length)
        {
            this = default(SourceSpan);
            this.Document = document;
            this.Offset = offset;
            this.Length = length;
            if (offset < 0 || offset >= document.Length)
            {
                throw new ArgumentException("offset is out of bounds.", nameof(offset));
            }
            if (length < 0 || offset + length > document.Length)
            {
                throw new ArgumentException("length is out of bounds.", nameof(length));
            }
        }

        /// <summary>
        /// Gets the document to which this span of source code refers.
        /// </summary>
        /// <returns>The source document.</returns>
        public SourceDocument Document { get; private set; }

        /// <summary>
        /// Gets the offset of the first character in this span.
        /// </summary>
        /// <returns>The offset of the first character in this span.</returns>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets this span's length, in characters.
        /// </summary>
        /// <returns>The length of this span.</returns>
        public int Length { get; private set; }

        /// <summary>
        /// Gets this span's contents as text.
        /// </summary>
        /// <returns>The span's contents.</returns>
        public string Text => Document.GetText(Offset, Length);
    }
}