namespace Pixie.Markup
{
    /// <summary>
    /// A centered, emphasized title block.
    /// </summary>
    public sealed class Title : Block
    {
        /// <summary>
        /// Creates a title block from a plain text string.
        /// </summary>
        /// <param name="title">The title text.</param>
        public Title(string title)
            : this(new Text(title))
        { }

        /// <summary>
        /// Creates a title block.
        /// </summary>
        /// <param name="title">The title contents.</param>
        public Title(Inline title)
        {
            this.Contents = title;
        }

        /// <summary>
        /// Gets the title contents.
        /// </summary>
        public Inline Contents { get; private set; }

        /// <summary>
        /// Lowers this title block to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            return new AlignBox(
                new Paragraph(
                    new DecorationSpan(
                        Contents,
                        TextDecoration.Bold,
                        DecorationSpan.UnifyDecorations)),
                Alignment.Center);
        }
    }
}
