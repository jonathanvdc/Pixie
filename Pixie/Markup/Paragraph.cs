namespace Pixie.Markup
{
    /// <summary>
    /// A paragraph of inline content.
    /// </summary>
    public sealed class Paragraph : Block
    {
        public Paragraph(Inline contents)
        {
            this.Contents = contents;
        }

        public Paragraph(params Inline[] contents)
            : this(new Sequence(contents))
        { }

        /// <summary>
        /// Gets the paragraph's inline contents.
        /// </summary>
        public Inline Contents { get; private set; }
    }
}
