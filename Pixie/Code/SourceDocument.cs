using System;
using System.IO;
using System.Text;

namespace Pixie.Code
{
    /// <summary>
    /// A document of source code that can be read from.
    /// </summary>
    public abstract class SourceDocument
    {
        /// <summary>
        /// Gets the document's identifier. This is typically
        /// a path.
        /// </summary>
        /// <returns>The document's identifier.</returns>
        public abstract string Identifier { get; }

        /// <summary>
        /// Gets the document's length, in characters.
        /// </summary>
        /// <returns>The document's length.</returns>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the number of lines in the document.
        /// </summary>
        /// <returns>The number of lines in the document.</returns>
        public abstract int LineCount { get; }

        /// <summary>
        /// Opens this source document at the given offset.
        /// </summary>
        /// <param name="offset">
        /// The offset to open the source document at.
        /// </param>
        /// <returns>A text reader.</returns>
        public abstract TextReader Open(int offset);

        /// <summary>
        /// Gets the (line index, line offset) pair that corresponds to the
        /// character in the document at a particular offset.
        /// </summary>
        /// <param name="offset">The offset to a character in the document.</param>
        /// <returns>A (line index, line offset) pair.</returns>
        public abstract GridPosition GetGridPosition(int offset);

        /// <summary>
        /// Gets the offset of the first character on a particular line.
        /// </summary>
        /// <param name="lineIndex">The zero-based index of the line to address.</param>
        /// <returns>
        /// The offset of the first character on the line, if it exists.
        /// Otherwise, the resulting offset is truncated.
        /// </returns>
        public abstract int GetLineOffset(int lineIndex);

        /// <summary>
        /// Gets a span of text in the document.
        /// </summary>
        /// <param name="offset">
        /// The offset of the first character to read.
        /// </param>
        /// <param name="length">
        /// The number of characters to read.
        /// </param>
        /// <returns>A span of text.</returns>
        public virtual string GetText(int offset, int length)
        {
            var buffer = new char[length];
            using (var reader = Open(offset))
            {
                reader.Read(buffer, 0, length);
            }
            return new StringBuilder().Append(buffer).ToString();
        }
    }
}
