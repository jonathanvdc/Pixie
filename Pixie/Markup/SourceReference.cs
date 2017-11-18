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
            GridPosition start,
            GridPosition end)
        {
            return new Text(
                documentIdentifier + ":" + (start.LineIndex + 1) + ":" + (start.Offset + 1));
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
                var startGridPos = Range.Document.GetGridPosition(Range.Offset);
                var endGridPos = Range.Document.GetGridPosition(
                    Range.Offset + Math.Max(Range.Length, 1) - 1);

                return Render(Range.Document.Identifier, startGridPos, endGridPos);
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
            GridPosition start,
            GridPosition end)
        {
            return new Text(
                documentIdentifier + "(" + (start.LineIndex + 1) + "," + (start.Offset + 1) + ")");
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
            GridPosition start,
            GridPosition end)
        {
            return new Text(
                documentIdentifier + " +" + (start.LineIndex + 1) + ":" + (start.Offset + 1));
        }
    }
}