using System;
using Pixie.Code;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that refers to a range of source code.
    /// </summary>
    public class SourceReference : MarkupNode
    {
        /// <summary>
        /// Creates a source reference from a range of source code.
        /// </summary>
        /// <param name="range">The range of source code.</param>
        public SourceReference(SourceSpan range)
        {
            this.Range = range;
        }

        /// <summary>
        /// Gets the span of source code this range refers to.
        /// </summary>
        /// <returns>A span of source code.</returns>
        public SourceSpan Range { get; private set; }

        /// <summary>
        /// Renders a source reference composed of a document
        /// identifier, a start position and an end position.
        /// </summary>
        /// <param name="documentIdentifier">A document identifier.</param>
        /// <param name="start">A start position.</param>
        /// <param name="end">An end position.</param>
        /// <returns>A markup node.</returns>
        protected virtual MarkupNode Render(
            string documentIdentifier,
            LineAndColumnPosition start,
            LineAndColumnPosition end)
        {
            return new Text(
                documentIdentifier + ":" + start.Line + ":" + start.Column);
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }

        /// <inheritdoc/>
        public sealed override MarkupNode Fallback
        {
            get
            {
                var start = Range.Document.GetPosition(Range.Start);
                var end = Range.Document.GetPosition(
                    Range.Start + Math.Max(Range.Length, 1) - 1);

                return Render(start.Identifier ?? Range.Document.Identifier, start, end);
            }
        }
    }

    /// <summary>
    /// A markup node that refers to a range of source code and renders it
    /// MSVC style.
    /// </summary>
    public sealed class MsvcSourceReference : SourceReference
    {
        /// <summary>
        /// Creates a source reference from a range of source code.
        /// </summary>
        /// <param name="range">The range of source code.</param>
        public MsvcSourceReference(SourceSpan range)
            : base(range)
        { }

        /// <inheritdoc/>
        protected override MarkupNode Render(
            string documentIdentifier,
            LineAndColumnPosition start,
            LineAndColumnPosition end)
        {
            return new Text(
                documentIdentifier + "(" + start.Line + "," + start.Column + ")");
        }
    }

    /// <summary>
    /// A markup node that refers to a range of source code and renders it
    /// Vi style.
    /// </summary>
    public sealed class ViSourceReference : SourceReference
    {
        /// <summary>
        /// Creates a source reference from a range of source code.
        /// </summary>
        /// <param name="range">The range of source code.</param>
        public ViSourceReference(SourceSpan range)
            : base(range)
        { }

        /// <inheritdoc/>
        protected override MarkupNode Render(
            string documentIdentifier,
            LineAndColumnPosition start,
            LineAndColumnPosition end)
        {
            return new Text(
                documentIdentifier + " +" + start.Line + ":" + start.Column);
        }
    }
}
