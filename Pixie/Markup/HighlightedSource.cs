using System;
using Pixie.Code;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that renders a highlighted region of source code.
    /// </summary>
    public sealed class HighlightedSource : Block
    {
        /// <summary>
        /// Creates a highlighted source block that focuses on a single region.
        /// </summary>
        /// <param name="focusRegion">The source region to focus on and highlight.</param>
        public HighlightedSource(SourceRegion focusRegion)
            : this(focusRegion, focusRegion)
        { }

        /// <summary>
        /// Creates a highlighted source block.
        /// </summary>
        /// <param name="highlightedRegion">The source region to highlight.</param>
        /// <param name="focusRegion">The source region to focus on.</param>
        public HighlightedSource(SourceRegion highlightedRegion, SourceRegion focusRegion)
        {
            if (highlightedRegion.Document != focusRegion.Document)
            {
                throw new ArgumentException(
                    "Highlighted and focused source regions must belong to the same document.");
            }

            this.HighlightedRegion = highlightedRegion;
            this.FocusRegion = focusRegion;
        }

        /// <summary>
        /// Gets the source region to highlight.
        /// </summary>
        public SourceRegion HighlightedRegion { get; private set; }

        /// <summary>
        /// Gets the source region to focus on.
        /// </summary>
        public SourceRegion FocusRegion { get; private set; }

        /// <summary>
        /// Gets the bounding span of the highlighted region.
        /// </summary>
        public SourceSpan HighlightedSpan => HighlightedRegion.BoundingSpan;

        /// <summary>
        /// Lowers this highlighted source block to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            return new Paragraph(new SourceReference(HighlightedSpan));
        }
    }
}
