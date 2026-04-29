using Pixie.Code;

namespace Pixie.Markup
{
    /// <summary>
    /// Inline source location text.
    /// </summary>
    public class SourceReference : Inline
    {
        /// <summary>
        /// Creates a source reference.
        /// </summary>
        /// <param name="range">The referenced source span.</param>
        public SourceReference(SourceSpan range)
        {
            this.Range = range;
        }

        /// <summary>
        /// Gets the referenced source span.
        /// </summary>
        public SourceSpan Range { get; private set; }

        /// <summary>
        /// Renders a source reference from resolved source coordinates.
        /// </summary>
        /// <param name="identifier">The source identifier.</param>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <returns>The rendered inline markup.</returns>
        protected virtual Inline Render(
            string identifier,
            LineAndColumnPosition start,
            LineAndColumnPosition end)
        {
            return new Text(
                identifier + ":" + start.Line + ":" + start.Column);
        }

        /// <summary>
        /// Lowers this source reference to simpler inline markup.
        /// </summary>
        /// <returns>The lowered inline markup.</returns>
        public override Inline Lower()
        {
            if (!Range.IsKnown)
            {
                return new Text("<unknown>");
            }

            var primary = Range.Resolve().PrimarySpan;
            var start = primary.Document.GetPosition(primary.Start);
            var end = primary.Document.GetPosition(primary.End);
            return Render(
                start.Identifier ?? primary.Document.Identifier,
                start,
                end);
        }
    }

    /// <summary>
    /// A source reference that includes both start and end coordinates.
    /// </summary>
    public sealed class SourceRangeReference : SourceReference
    {
        /// <summary>
        /// Creates a source range reference.
        /// </summary>
        /// <param name="range">The referenced source span.</param>
        public SourceRangeReference(SourceSpan range)
            : base(range)
        { }

        /// <summary>
        /// Renders a source range reference from resolved source coordinates.
        /// </summary>
        /// <param name="identifier">The source identifier.</param>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <returns>The rendered inline markup.</returns>
        protected override Inline Render(
            string identifier,
            LineAndColumnPosition start,
            LineAndColumnPosition end)
        {
            return new Text(
                identifier + ":" + start.Line + ":" + start.Column
                + "-" + end.Line + ":" + end.Column);
        }
    }
}
