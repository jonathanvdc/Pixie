namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that specifies the color with which its contents
    /// are printed.
    /// </summary>
    public sealed class ColorSpan : ContainerNode
    {
        /// <summary>
        /// Creates a color span from the given contents, foreground color
        /// and background color.
        /// </summary>
        /// <param name="contents">The contents to print.</param>
        /// <param name="foregroundColor">The foreground color to print the contents with.</param>
        /// <param name="backgroundColor">The background color to print the contents with.</param>
        public ColorSpan(
            MarkupNode contents,
            Color foregroundColor,
            Color backgroundColor)
            : base(contents)
        {
            this.ForegroundColor = foregroundColor;
            this.BackgroundColor = backgroundColor;
        }

        /// <summary>
        /// Creates a color span from the given contents and foreground color.
        /// </summary>
        /// <param name="contents">The contents to print.</param>
        /// <param name="foregroundColor">The foreground color to print the contents with.</param>
        public ColorSpan(
            MarkupNode contents,
            Color foregroundColor)
            : this(contents, foregroundColor, Colors.Transparent)
        { }

        /// <summary>
        /// Gets the foreground color that is applied to this span's contents.
        /// </summary>
        /// <returns>The foreground color.</returns>
        public Color ForegroundColor { get; private set; }

        /// <summary>
        /// Gets the background color that is applied to this span's contents.
        /// </summary>
        /// <returns>The background color.</returns>
        public Color BackgroundColor { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback => Contents;
    }
}