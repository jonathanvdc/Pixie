namespace Pixie.Markup
{
    /// <summary>
    /// A block that prefixes the first line of its contents and aligns
    /// subsequent lines with the content that follows the prefix.
    /// </summary>
    public sealed class PrefixBox : Block
    {
        public PrefixBox(Inline prefix, Block contents)
        {
            this.Prefix = prefix;
            this.Contents = contents;
        }

        public Inline Prefix { get; private set; }

        public Block Contents { get; private set; }

        public override Block Lower()
        {
            return new Paragraph(Prefix);
        }
    }
}
