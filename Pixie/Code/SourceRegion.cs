using System;
using System.Collections.Generic;

namespace Pixie.Code
{
    /// <summary>
    /// Specifies a sparse region of text in a document.
    /// </summary>
    public sealed class SourceRegion
    {
        private SourceRegion()
        { }

        /// <summary>
        /// Creates a source region from a span.
        /// </summary>
        /// <param name="span">A span of text in a document.</param>
        public SourceRegion(SourceSpan span)
        {
            this.Document = span.Document;
            this.minIndex = span.Offset;
            this.maxIndex = span.Offset + span.Length;
            this.regionIndices = new HashSet<int>();
            for (int i = 0; i < span.Length; i++)
            {
                this.regionIndices.Add(span.Offset + i);
            }
        }

        private int minIndex;
        private int maxIndex;
        private HashSet<int> regionIndices;

        /// <summary>
        /// Gets the document that is the context for this region.
        /// </summary>
        /// <returns>The source document.</returns>
        public SourceDocument Document { get; private set; }

        /// <summary>
        /// Gets the offset to the start of this region.
        /// </summary>
        /// <returns>The offset to the start of this region.</returns>
        public int StartOffset => minIndex;

        /// <summary>
        /// Gets the offset to the end of this region: the index of the
        /// first character that does not belong to this region.
        /// </summary>
        /// <returns>The offset to the end of this region.</returns>
        public int EndOffset => maxIndex;

        /// <summary>
        /// Gets the length of this region: the difference between the end
        /// and start of this region.
        /// </summary>
        /// <returns>The length of the region.</returns>
        public int Length => EndOffset - StartOffset;

        /// <summary>
        /// Checks if this region contains the character at the given offset.
        /// </summary>
        /// <param name="offset">The offset of a character.</param>
        /// <returns>
        /// <c>true</c> if this region contains the character at the offset; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int offset)
        {
            return regionIndices.Contains(offset);
        }

        /// <summary>
        /// Computes the union of this region and another region
        /// </summary>
        /// <param name="region">A region of text in the same document.</param>
        /// <returns>A new source region.</returns>
        public SourceRegion Union(SourceRegion region)
        {
            if (Document != region.Document)
            {
                throw new ArgumentException(
                    "Regions are not in the same document.",
                    nameof(region));
            }

            var newRegion = new SourceRegion();
            newRegion.Document = Document;
            newRegion.regionIndices = new HashSet<int>(regionIndices);
            newRegion.regionIndices.UnionWith(region.regionIndices);
            newRegion.minIndex = Math.Min(region.minIndex, minIndex);
            newRegion.maxIndex = Math.Max(region.maxIndex, maxIndex);
            return region;
        }

        /// <summary>
        /// Computes the union of this region and a span.
        /// </summary>
        /// <param name="span">A span of text in the same document.</param>
        /// <returns>A new source region.</returns>
        public SourceRegion Union(SourceSpan span)
        {
            if (Document != span.Document)
            {
                throw new ArgumentException(
                    "Region and span are not in the same document.",
                    nameof(span));
            }

            var region = new SourceRegion();
            region.Document = Document;
            region.regionIndices = new HashSet<int>(regionIndices);
            for (int i = 0; i < span.Length; i++)
            {
                this.regionIndices.Add(span.Offset + i);
            }
            region.minIndex = Math.Min(span.Offset, minIndex);
            region.maxIndex = Math.Max(span.Offset + span.Length, maxIndex);
            return region;
        }
    }
}