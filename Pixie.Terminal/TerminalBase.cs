namespace Pixie.Terminal
{
    /// <summary>
    /// A base type for a terminal which can be used by Pixie
    /// to render markup nodes as text.
    /// </summary>
    public abstract class TerminalBase
    {
        /// <summary>
        /// Prints a string of characters to the terminal.
        /// </summary>
        /// <param name="text">The text to print.</param>
        public abstract void Write(string text);

        /// <summary>
        /// Writes an end-of-line sequence to the terminal.
        /// </summary>
        public abstract void WriteLine();
    }
}