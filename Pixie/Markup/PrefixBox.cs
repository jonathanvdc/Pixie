using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A box that prefixes the first line of its contents and then
    /// adjusts the margin to align subsequent lines with the start
    /// of the prefix.
    /// </summary>
    public sealed class PrefixBox : MarkupNode
    {
        /// <summary>
        /// Creates a prefix-box from a body node.
        /// </summary>
        /// <param name="prefix">A prefix for the box.</param>
        /// <param name="contents">A box's contents.</param>
        public PrefixBox(MarkupNode prefix, MarkupNode contents)
        {
            this.Prefix = prefix;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the prefix-box's prefix node.
        /// </summary>
        /// <returns>The prefix.</returns>
        public MarkupNode Prefix { get; private set; }

        /// <summary>
        /// Gets the prefix-box's body.
        /// </summary>
        /// <returns>The prefix-box's body.</returns>
        public MarkupNode Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(Prefix, Contents);

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            var newPrefix = mapping(Prefix);
            var newContents = mapping(Contents);

            if (newPrefix == Prefix
                && newContents == Contents)
            {
                return this;
            }
            else
            {
                return new PrefixBox(newPrefix, newContents);
            }
        }
    }
}