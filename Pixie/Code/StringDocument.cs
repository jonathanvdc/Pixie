using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pixie.Code
{
    /// <summary>
    /// A document of source code that is stored as a string.
    /// </summary>
    public sealed class StringDocument : SourceDocument
    {
        /// <summary>
        /// Creates a string document from an identifier and a contents
        /// string.
        /// </summary>
        /// <param name="identifier">The document's identifier.</param>
        /// <param name="contents">The document's contents.</param>
        public StringDocument(string identifier, string contents)
        {
            this.ident = identifier;
            this.Contents = contents;
            this.lineOffsets = ComputeLineOffsets(contents);
        }

        private string ident;

        /// <summary>
        /// Gets the string that defines this string document's contents.
        /// </summary>
        /// <returns>The document's contents string.</returns>
        public string Contents { get; private set; }

        private List<int> lineOffsets;

        /// <inheritdoc/>
        public override string Identifier => ident;

        /// <inheritdoc/>
        public override int Length => Contents.Length;

        /// <inheritdoc/>
        public override TextReader Open(int offset)
        {
            var reader = new StringReader(Contents);
            if (offset > 0)
            {
                int bufSize = 1024;
                var buffer = new char[bufSize];
                while (offset > 0)
                {
                    var readCount = Math.Min(offset, bufSize);
                    reader.Read(buffer, 0, readCount);
                }
            }
            return reader;
        }

        /// <inheritdoc/>
        public override string GetText(int offset, int length)
        {
            return Contents.Substring(offset, length);
        }

        /// <inheritdoc/>
        public override GridPosition GetGridPosition(int offset)
        {
            // Choose the last line index such that `offset` is greater
            // than or equal to the offset of the line start.

            int lineIndex = 0;
            for (int i = 0; i < lineOffsets.Count; i++)
            {
                if (lineOffsets[i] > offset)
                {
                    break;
                }

                lineIndex = i;
            }

            int lineStartOffset = lineOffsets[lineIndex];

            return new GridPosition(lineIndex, offset - lineStartOffset);
        }

        /// <inheritdoc/>
        public override int GetLineOffset(int lineIndex)
        {
            if (lineIndex < 0)
                return 0;
            else if (lineIndex >= lineOffsets.Count)
                return Length;
            else
                return lineOffsets[lineIndex];
        }

        /// <summary>
        /// Computes the offsets at which new lines start in a particular string.
        /// </summary>
        /// <param name="str">The string to examine.</param>
        /// <returns>The offsets at which new lines start.</returns>
        private static List<int> ComputeLineOffsets(string str)
        {
            var results = new List<int>();
            results.Add(0);
            int i = 0;
            while (i < str.Length)
            {
                // Skip to the next newline.
                i = str.IndexOf('\n', i);

                if (i < 0)
                {
                    break;
                }

                i++;

                // Skip carriage returns.
                while (i < str.Length && str.Substring(i, 1) == "\r")
                {
                    i++;
                }

                // Log the first character of the new line.
                results.Add(i);
            }

            return results;
        }
    }
}
