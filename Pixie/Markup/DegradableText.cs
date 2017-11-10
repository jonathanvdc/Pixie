namespace Pixie.Markup
{
    /// <summary>
    /// A text node that can be degraded gracefully to a fallback
    /// if the original text cannot be rendered.
    /// </summary>
    public sealed class DegradableText : MarkupNode
    {
        /// <summary>
        /// Creates a degradable text node from a main string
        /// and a fallback string.
        /// </summary>
        /// <param name="text">The main string.</param>
        /// <param name="fallback">
        /// The fallback string, which is rendered when the main string can't be rendered.
        /// </param>
        public DegradableText(string text, string fallback)
            : this(text, new Text(fallback))
        { }

        /// <summary>
        /// Creates a degradable text node from a main string
        /// and a fallback node.
        /// </summary>
        /// <param name="text">The main string.</param>
        /// <param name="fallback">
        /// The fallback node, which is rendered when the main string can't be rendered.
        /// </param>
        public DegradableText(string text, MarkupNode fallback)
        {
            this.Contents = text;
            this.fallbackNode = fallback;
        }

        /// <summary>
        /// Gets the text this node consists of.
        /// </summary>
        /// <returns>A text string.</returns>
        public string Contents { get; private set; }

        private MarkupNode fallbackNode;

        /// <inheritdoc/>
        public override MarkupNode Fallback => fallbackNode;
    }
}