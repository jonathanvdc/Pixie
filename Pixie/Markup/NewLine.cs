using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A node that produces a new-line sequence.
    /// </summary>
    public sealed class NewLine : MarkupNode
    {
        private NewLine() { }

        /// <summary>
        /// An instance of a newline node.
        /// </summary>
        public static readonly NewLine Instance = new NewLine();

        /// <inheritdoc/>
        public override MarkupNode Fallback => new Text(Environment.NewLine);

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }
    }
}