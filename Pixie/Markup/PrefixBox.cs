namespace Pixie.Markup
{
    /// <summary>
    /// A block that prefixes the first line of its contents and aligns
    /// subsequent lines with the content that follows the prefix.
    /// </summary>
    public sealed class PrefixBox : Block
    {
        /// <summary>
        /// Creates a prefixed block.
        /// </summary>
        /// <param name="prefix">The prefix applied to the first line.</param>
        /// <param name="contents">The block contents.</param>
        public PrefixBox(Inline prefix, Block contents)
        {
            this.Prefix = prefix;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the prefix applied to the first line.
        /// </summary>
        public Inline Prefix { get; }

        /// <summary>
        /// Gets the block contents.
        /// </summary>
        public Block Contents { get; }

        /// <summary>
        /// Lowers this prefixed block to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            return new Paragraph(Prefix);
        }
    }
}
