using System;
using System.Collections.Generic;
using System.Globalization;
using Pixie.Markup;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that buffers lines, aligns them and writes them to
    /// another terminal.
    /// </summary>
    public sealed class AlignedTerminal : TerminalBase
    {
        public AlignedTerminal(Alignment alignment, TerminalBase unalignedTerminal)
        {
            this.Alignment = alignment;
            this.UnalignedTerminal = unalignedTerminal;
            Reset();
        }

        /// <summary>
        /// Tells how this terminal aligns lines.
        /// </summary>
        /// <returns>The alignment this terminal uses.</returns>
        public Alignment Alignment { get; private set; }

        /// <summary>
        /// Gets the unaligned terminal to which aligned lines are written.
        /// </summary>
        /// <returns>A terminal.</returns>
        public TerminalBase UnalignedTerminal { get; private set; }

        /// <inheritdoc/>
        public override int Width => UnalignedTerminal.Width;

        private List<Action<TerminalBase>> commandBuffer;

        private int lineLength;

        /// <inheritdoc/>
        public override void Write(string text)
        {
            lineLength += new StringInfo(text).LengthInTextElements;
            commandBuffer.Add(new TerminalWriteCommand(text).Run);
        }

        /// <inheritdoc/>
        public override void WriteLine()
        {
            Flush();
            UnalignedTerminal.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteSeparator(int lineCount)
        {
            if (lineCount > 0)
            {
                Flush();
            }
            UnalignedTerminal.WriteSeparator(lineCount);
        }

        /// <summary>
        /// Flushes this aligned terminal's buffer to the inner
        /// terminal.
        /// </summary>
        public void Flush()
        {
            // Write padding.
            int padding = GetLeftPaddingSize(
                Alignment, lineLength, UnalignedTerminal.Width);
            for (int i = 0; i < padding; i++)
            {
                UnalignedTerminal.Write(' ');
            }

            // Flush the command buffer.
            for (int i = 0; i < commandBuffer.Count; i++)
            {
                commandBuffer[i](UnalignedTerminal);
            }

            // Reset the command buffer and line length.
            Reset();
        }

        private void Reset()
        {
            commandBuffer = new List<Action<TerminalBase>>();
            lineLength = 0;
        }

        private static int GetLeftPaddingSize(
            Alignment alignment,
            int lineLength,
            int terminalWidth)
        {
            switch (alignment)
            {
                case Alignment.Left:
                    return 0;
                case Alignment.Right:
                    return Math.Max(terminalWidth - lineLength, 0);
                case Alignment.Center:
                    return GetLeftPaddingSize(
                        Alignment.Right, lineLength, terminalWidth) / 2;
                default:
                    throw new NotSupportedException(
                        "Unsupported alignment type: " + alignment);
            }
        }
    }

    internal sealed class TerminalWriteCommand
    {
        public TerminalWriteCommand(string text)
        {
            this.Text = text;
        }

        public string Text { get; private set; }

        public void Run(TerminalBase terminal)
        {
            terminal.Write(Text);
        }
    }
}