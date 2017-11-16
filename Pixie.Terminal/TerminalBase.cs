namespace Pixie.Terminal
{
    /// <summary>
    /// A base type for a terminal which can be used by Pixie
    /// to render markup nodes as text.
    /// </summary>
    public abstract class TerminalBase
    {
        /// <summary>
        /// Gets the style manager for this terminal.
        /// </summary>
        /// <returns>The style manager.</returns>
        public abstract StyleManager Style { get; }

        /// <summary>
        /// Gets the terminal's width, in characters.
        /// </summary>
        /// <returns>The terminal's width.</returns>
        public abstract int Width { get; }

        /// <summary>
        /// Tells if this terminal can render a particular
        /// character string.
        /// </summary>
        /// <param name="text">A string to render.</param>
        /// <returns>
        /// <c>true</c> if the text can be rendered; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanRender(string text);

        /// <summary>
        /// Prints a string of characters to the terminal.
        /// </summary>
        /// <param name="text">The text to print.</param>
        public abstract void Write(string text);

        /// <summary>
        /// Prints an end-of-line sequence to the terminal.
        /// </summary>
        public abstract void WriteLine();

        /// <summary>
        /// Separates the current text from the next
        /// by at least the given number of newlines.
        /// </summary>
        /// <param name="lineCount">The minimum number of newlines.</param>
        public abstract void WriteSeparator(int lineCount);

        /// <summary>
        /// Prints a character to the terminal.
        /// </summary>
        /// <param name="character">The character to write.</param>
        public virtual void Write(char character)
        {
            Write(character.ToString());
        }

        /// <summary>
        /// Gets the first renderable string in a sequence of strings.
        /// If the sequence is empty or no string is renderable, <c>null</c>
        /// is returned.
        /// </summary>
        /// <param name="options">A sequence of strings.</param>
        /// <returns>
        /// The first renderable string in a sequence of strings.
        /// If the sequence is empty or no string is renderable, <c>null</c>
        /// is returned.
        /// </returns>
        public string GetFirstRenderableString(params string[] options)
        {
            foreach (var opt in options)
            {
                if (CanRender(opt))
                {
                    return opt;
                }
            }
            return null;
        }
    }
}