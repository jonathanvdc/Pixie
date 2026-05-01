namespace Pixie.Markup
{
    /// <summary>
    /// A neutral block of inline content that participates in document flow
    /// without adding paragraph spacing.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Div"/> for labels, terms, compact headings, and other
    /// inline-flow content that should not carry the prose spacing implied by
    /// <see cref="Paragraph"/>.
    /// </remarks>
    public sealed class Div : Block
    {
        /// <summary>
        /// Creates a neutral block of inline content.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        public Div(Inline contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Creates a neutral block from a parameter list of inline nodes.
        /// </summary>
        /// <param name="contents">The block contents.</param>
        public Div(params Inline[] contents)
            : this(new Sequence(contents))
        { }

        /// <summary>
        /// Gets the inline contents of this neutral block.
        /// </summary>
        public Inline Contents { get; }
    }
}
