using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that renders a string of text.
    /// </summary>
    public sealed class Text : MarkupNode
    {
        /// <summary>
        /// Creates a text node from a string.
        /// </summary>
        /// <param name="text">A text string.</param>
        public Text(string text)
        {
            this.Contents = text;
        }

        /// <summary>
        /// Gets the text this node consists of.
        /// </summary>
        /// <returns>A text string.</returns>
        public string Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback => null;

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }
    }
}