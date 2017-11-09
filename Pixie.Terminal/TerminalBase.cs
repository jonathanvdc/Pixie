namespace Pixie.Terminal
{
    /// <summary>
    /// A base type for a terminal which can be used by Pixie
    /// to render markup nodes as text.
    /// </summary>
    public abstract class TerminalBase
    {
        /// <summary>
        /// Gets the terminal's width, in characters.
        /// </summary>
        /// <returns>The terminal's width.</returns>
        public abstract int Width { get; }

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
    }
}