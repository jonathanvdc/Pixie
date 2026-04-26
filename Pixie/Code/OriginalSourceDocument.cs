using System;
using System.Collections.Generic;

namespace Pixie.Code
{
    /// <summary>
    /// Represents a user-authored source document and owns its display coordinate mapping.
    /// </summary>
    /// <remarks>
    /// Original documents are the final diagnostic coordinate space. Mapped or preprocessed
    /// documents ultimately resolve their locations to <see cref="OriginalSourceSpan"/> values
    /// backed by instances of this type.
    /// </remarks>
    public abstract class OriginalSourceDocument : SourceDocument
    {
        private readonly string identifier;
        private readonly Lazy<List<int>> lazyLineOffsets;

        /// <summary>
        /// Creates an original source document from an identifier and contents string.
        /// </summary>
        /// <param name="identifier">The document's identifier.</param>
        public OriginalSourceDocument(string identifier)
        {
            this.identifier = identifier;
            this.lazyLineOffsets = new Lazy<List<int>>(ComputeLineOffsets);
        }

        private IReadOnlyList<int> lineOffsets => lazyLineOffsets.Value;

        /// <inheritdoc/>
        public override string Identifier => identifier;

        /// <inheritdoc/>
        public override int LineCount => lineOffsets.Count;

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

        private List<int> ComputeLineOffsets()
        {
            var results = new List<int>();
            results.Add(0);
            int i = 0;
            var str = GetText(0, Length);
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
