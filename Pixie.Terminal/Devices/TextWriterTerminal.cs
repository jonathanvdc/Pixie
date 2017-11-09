using System;
using System.IO;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that writes to a text writer.
    /// </summary>
    public class TextWriterTerminal : TerminalBase
    {
        public TextWriterTerminal(TextWriter writer)
        {
            this.Writer = writer;
        }

        public TextWriter Writer { get; private set; }

        /// <inheritdoc/>
        public override void Write(string text)
        {
            Writer.Write(text);
        }

        /// <inheritdoc/>
        public override void WriteLine()
        {
            Writer.WriteLine();
        }
    }
}