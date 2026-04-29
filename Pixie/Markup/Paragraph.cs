namespace Pixie.Markup
{
    /// <summary>
    /// A paragraph of inline content.
    /// </summary>
    public sealed class Paragraph : Block
    {
        /// <summary>
        /// Creates a paragraph of inline content.
        /// </summary>
        /// <param name="contents">The paragraph contents.</param>
        public Paragraph(Inline contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Creates a paragraph from a parameter list of inline nodes.
        /// </summary>
        /// <param name="contents">The paragraph contents.</param>
        public Paragraph(params Inline[] contents)
            : this(new Sequence(contents))
        { }

        /// <summary>
        /// Gets the paragraph's inline contents.
        /// </summary>
        public Inline Contents { get; }
    }
}
