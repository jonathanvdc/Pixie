using System;
using System.Collections.Generic;
using System.IO;

namespace Pixie.Code
{
    /// <summary>
    /// A user-authored source document that owns its diagnostic coordinate mapping.
    /// </summary>
    public class OriginalSourceDocument : SourceDocument
    {
        private readonly string identifier;
        private readonly List<int> lineOffsets;

        /// <summary>
        /// Creates an original source document from an identifier and contents string.
        /// </summary>
        /// <param name="identifier">The document's identifier.</param>
        /// <param name="contents">The document's contents.</param>
        public OriginalSourceDocument(string identifier, string contents)
        {
            this.identifier = identifier;
            this.Contents = contents ?? string.Empty;
            this.lineOffsets = ComputeLineOffsets(this.Contents);
        }

        /// <summary>
        /// Gets the string that defines this source document's contents.
        /// </summary>
        /// <returns>The document's contents string.</returns>
        public string Contents { get; private set; }

        /// <inheritdoc/>
        public override string Identifier => identifier;

        /// <inheritdoc/>
        public override int Length => Contents.Length;

        /// <inheritdoc/>
        public override int LineCount => lineOffsets.Count;

        /// <inheritdoc/>
        public override TextReader Open(int offset)
        {
            return new StringReader(GetText(offset, Length - offset));
        }

        /// <inheritdoc/>
        public override string GetText(int offset, int length)
        {
            return Contents.Substring(offset, length);
        }

        /// <inheritdoc/>
        public override SourcePosition GetPosition(int offset)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            else if (offset > Length)
            {
                offset = Length;
            }

            int lo = 0;
            int hi = lineOffsets.Count - 1;
            while (lo < hi)
            {
                int mid = lo + (hi - lo + 1) / 2;
                if (lineOffsets[mid] <= offset)
                {
                    lo = mid;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return new SourcePosition(
                Identifier,
                lo + 1,
                offset - lineOffsets[lo] + 1);
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

        /// <inheritdoc/>
        public override ResolvedSourceSpan ResolveSpan(int start, int length)
        {
            if (start < 0 || start > Length)
            {
                throw new ArgumentException("start is out of bounds.", nameof(start));
            }
            if (length < 0 || start + length > Length)
            {
                throw new ArgumentException("length is out of bounds.", nameof(length));
            }

            var span = new OriginalSourceSpan(this, start, length);
            return new ResolvedSourceSpan(span, new[] { span });
        }

        private static List<int> ComputeLineOffsets(string str)
        {
            var results = new List<int>();
            results.Add(0);
            int i = 0;
            while (i < str.Length)
            {
                i = str.IndexOf('\n', i);

                if (i < 0)
                {
                    break;
                }

                i++;
                results.Add(i);
            }

            return results;
        }
    }
}
