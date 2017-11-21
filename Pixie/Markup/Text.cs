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

        /// <summary>
        /// Tells if a markup node is certainly an empty node.
        /// </summary>
        /// <param name="node">The markup node to examine.</param>
        /// <returns>
        /// <c>true</c> if the node is certainly an empty node; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(MarkupNode node)
        {
            if (node is Text)
            {
                return string.IsNullOrEmpty(((Text)node).Contents);
            }
            else if (node is Sequence)
            {
                var seq = (Sequence)node;
                if (seq.Contents.Count == 0)
                    return true;
                else if (seq.Contents.Count == 1)
                    return IsEmpty(seq.Contents[0]);
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Fallback => null;

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }
    }
}