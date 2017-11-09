namespace Pixie
{
    /// <summary>
    /// A markup node that renders a string of text.
    /// </summary>
    public sealed class TextNode : MarkupNode
    {
        /// <summary>
        /// Creates a text node from a string.
        /// </summary>
        /// <param name="text">A text string.</param>
        public TextNode(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the text this node consists of.
        /// </summary>
        /// <returns>A text string.</returns>
        public string Text { get; private set; }
    }
}