namespace Pixie.Terminal
{
    /// <summary>
    /// A base class for objects that manage the style of the text printed
    /// by a terminal.
    /// </summary>
    public abstract class StyleManager
    {
        /// <summary>
        /// Pushes a foreground color onto the style stack.
        /// </summary>
        /// <param name="color">The foreground color to push.</param>
        public abstract void PushForegroundColor(Color color);

        /// <summary>
        /// Pushes a background color onto the style stack.
        /// </summary>
        /// <param name="color">The background color to push.</param>
        public abstract void PushBackgroundColor(Color color);

        /// <summary>
        /// Pops an entry from the style stack.
        /// </summary>
        public abstract void PopStyle();
    }
}