namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that specifies the color with which its contents
    /// are printed.
    /// </summary>
    public sealed class ColorSpan : InlineContainer
    {
        /// <summary>
        /// Creates a color span from the given contents, foreground color
        /// and background color.
        /// </summary>
        /// <param name="contents">The contents to print.</param>
        /// <param name="foregroundColor">The foreground color to print the contents with.</param>
        /// <param name="backgroundColor">The background color to print the contents with.</param>
        public ColorSpan(
            Inline contents,
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
            Inline contents,
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
        public override Inline Lower()
        {
            return Contents;
        }

        /// <inheritdoc/>
        public override InlineContainer WithContents(Inline newContents)
        {
            return new ColorSpan(newContents, ForegroundColor, BackgroundColor);
        }
    }
}
