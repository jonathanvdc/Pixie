using System;
using Pixie.Code;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that renders a highlighted region of source code.
    /// </summary>
    public sealed class HighlightedSource : Block
    {
        public HighlightedSource(SourceRegion focusRegion)
            : this(focusRegion, focusRegion)
        { }

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

        public SourceRegion HighlightedRegion { get; private set; }

        public SourceRegion FocusRegion { get; private set; }

        public SourceSpan HighlightedSpan => HighlightedRegion.BoundingSpan;

        public override Block Lower()
        {
            return new Paragraph(new SourceReference(HighlightedSpan));
        }
    }
}
