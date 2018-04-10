using System.Text;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that puts quotation signs around another node.
    /// </summary>
    public sealed class Quotation : ContainerNode
    {
        /// <summary>
        /// Creates a quotation node from a quoted contents
        /// node.
        /// </summary>
        /// <param name="contents">
        /// The contents to quote.
        /// </param>
        public Quotation(MarkupNode contents)
            : this(contents, 1)
        { }

        /// <summary>
        /// Creates a quotation node from a quoted contents
        /// node and an amount of quotation signs to put on
        /// each side of the contents.
        /// </summary>
        /// <param name="contents">
        /// The contents to quote.
        /// </param>
        /// <param name="numberOfQuotes">
        /// The number of quotation signs on each side of
        /// the contents.
        /// </param>
        public Quotation(MarkupNode contents, int numberOfQuotes)
            : base(contents)
        {
            this.NumberOfQuotes = numberOfQuotes;
        }

        /// <summary>
        /// Gets the number of quotation signs to put on
        /// each side of the quoted contents.
        /// </summary>
        /// <returns>The number of quotation signs.</returns>
        public int NumberOfQuotes { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback
        {
            get
            {
                return new Sequence(
                    new DegradableText(
                        BuildQuotationSign(NumberOfQuotes, '‘', '“', true),
                        BuildQuotationSign(NumberOfQuotes, '\'', '"', true)),
                    Contents,
                    new DegradableText(
                        BuildQuotationSign(NumberOfQuotes, '’', '”', false),
                        BuildQuotationSign(NumberOfQuotes, '\'', '"', false)));
            }
        }

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new Quotation(newContents, NumberOfQuotes);
        }

        private static string BuildQuotationSign(
            int NumberOfQuotes,
            char SingleQuote,
            char DoubleQuote,
            bool PutDoubleQuotesFirst)
        {
            var sb = new StringBuilder();
            if (PutDoubleQuotesFirst)
            {
                sb.Append(DoubleQuote, NumberOfQuotes / 2);
            }
            sb.Append(SingleQuote, NumberOfQuotes % 2);
            if (!PutDoubleQuotesFirst)
            {
                sb.Append(DoubleQuote, NumberOfQuotes / 2);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates a bold quotation node.
        /// </summary>
        /// <param name="contents">The contents to quote.</param>
        /// <returns>A bold quotation node.</returns>
        public static MarkupNode CreateBoldQuotation(MarkupNode contents)
        {
            return new Quotation(DecorationSpan.MakeBold(contents));
        }

        /// <summary>
        /// Creates a bold quotation node.
        /// </summary>
        /// <param name="contents">The contents to quote.</param>
        /// <returns>A bold quotation node.</returns>
        public static MarkupNode CreateBoldQuotation(string contents)
        {
            return CreateBoldQuotation(new Text(contents));
        }
    }
}