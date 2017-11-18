using System;
using Pixie.Code;

namespace Pixie.Markup
{
    /// <summary>
    /// A node that points to a region of source code.
    /// </summary>
    public sealed class HighlightedSource : MarkupNode
    {
        /// <summary>
        /// Creates a highlighted snippet of source from a
        /// region of source code to highlight.
        /// </summary>
        /// <param name="highlightRegion">
        /// The region of source that is to be highlighted.
        /// </param>
        public HighlightedSource(
            SourceRegion highlightRegion)
            : this(
                highlightRegion,
                new SourceRegion(
                    new SourceSpan(
                        highlightRegion.Document,
                        highlightRegion.StartOffset,
                        1)))
        { }

        /// <summary>
        /// Creates a highlighted snippet of source from a
        /// region of source code to highlight and a region
        /// of source code to focus on.
        /// </summary>
        /// <param name="highlightRegion">
        /// The region of source that is to be highlighted.
        /// </param>
        /// <param name="focusRegion">
        /// The region of source that users should focus on
        /// specifically.
        /// </param>
        public HighlightedSource(
            SourceRegion highlightRegion,
            SourceRegion focusRegion)
        {
            if (highlightRegion.Document != focusRegion.Document)
            {
                throw new ArgumentException(
                    "Highlight and focus regions are in different documents.",
                    nameof(focusRegion));
            }

            this.HighlightRegion = highlightRegion;
            this.FocusRegion = focusRegion;
        }

        /// <summary>
        /// Gets the region of source that is to be highlighted.
        /// </summary>
        /// <returns>The highlighted region of source.</returns>
        public SourceRegion HighlightRegion { get; private set; }

        /// <summary>
        /// Gets the region of source that users should focus
        /// on specifically. It shouldn't be longer than a few
        /// characters.
        /// </summary>
        /// <returns>The focus region.</returns>
        public SourceRegion FocusRegion { get; private set; }

        /// <summary>
        /// Gets a source span that encompasses all characters
        /// highlighted by this node.
        /// </summary>
        public SourceSpan HighlightedSpan =>
            new SourceSpan(
                HighlightRegion.Document,
                HighlightRegion.StartOffset,
                HighlightRegion.Length);

        /// <inheritdoc/>
        public override MarkupNode Fallback => 
            new Paragraph(
                new Sequence(
                    new Text("At "),
                    new SourceReference(HighlightedSpan),
                    new Text(".")));

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }
    }
}