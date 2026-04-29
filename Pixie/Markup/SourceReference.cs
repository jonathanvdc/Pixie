using Pixie.Code;

namespace Pixie.Markup
{
    /// <summary>
    /// Inline source location text.
    /// </summary>
    public class SourceReference : Inline
    {
        public SourceReference(SourceSpan range)
        {
            this.Range = range;
        }

        public SourceSpan Range { get; private set; }

        protected virtual Inline Render(
            string identifier,
            LineAndColumnPosition start,
            LineAndColumnPosition end)
        {
            return new Text(
                identifier + ":" + start.Line + ":" + start.Column);
        }

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
        public SourceRangeReference(SourceSpan range)
            : base(range)
        { }

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
