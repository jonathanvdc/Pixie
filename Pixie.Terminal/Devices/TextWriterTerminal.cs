using System;
using System.IO;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that writes to a text writer.
    /// </summary>
    public class TextWriterTerminal : OutputTerminalBase
    {
        public TextWriterTerminal(TextWriter writer, int width)
        {
            this.Writer = writer;
            this.termWidth = width;
        }

        public TextWriter Writer { get; private set; }

        private int termWidth;

        public override int Width => termWidth;

        /// <inheritdoc/>
        public override void Write(string text)
        {
            EndSeparator();
            Writer.Write(text);
        }

        /// <inheritdoc/>
        protected override void WriteLineImpl()
        {
            Writer.WriteLine();
        }

        /// <inheritdoc/>
        public override void Write(char character)
        {
            EndSeparator();
            Writer.Write(character);
        }
    }
}