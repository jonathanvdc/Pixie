namespace Pixie.Markup
{
    /// <summary>
    /// A node that produces a new-line sequence.
    /// </summary>
    public sealed class NewLine : Inline
    {
        private NewLine() { }

        /// <summary>
        /// An instance of a newline node.
        /// </summary>
        public static readonly NewLine Instance = new NewLine();

    }
}
