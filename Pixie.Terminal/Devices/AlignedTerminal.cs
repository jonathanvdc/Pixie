using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Pixie.Markup;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that buffers lines, aligns them, word-wraps them
    /// and writes them to another terminal.
    /// </summary>
    public sealed class AlignedTerminal : TerminalBase
    {
        public AlignedTerminal(
            TerminalBase unalignedTerminal,
            Alignment alignment,
            WrappingStrategy wrapping,
            string leftPadding,
            int width)
        {
            this.Alignment = alignment;
            this.Wrapping = wrapping;
            this.UnalignedTerminal = unalignedTerminal;
            this.LeftPadding = leftPadding;
            this.width = width;
            if (width <= 0)
            {
                throw new ArgumentException(nameof(width));
            }
            Reset();
        }

        /// <summary>
        /// Tells how this terminal aligns lines.
        /// </summary>
        /// <returns>The alignment this terminal uses.</returns>
        public Alignment Alignment { get; private set; }

        /// <summary>
        /// Gets the padding that is added to the left of each
        /// non-empty line.
        /// </summary>
        /// <value>The left padding.</value>
        public string LeftPadding { get; private set; }

        /// <summary>
        /// Gets the wrapping strategy for this aligned terminal.
        /// </summary>
        /// <returns>The wrapping strategy.</returns>
        public WrappingStrategy Wrapping { get; private set; }

        /// <summary>
        /// Gets the unaligned terminal to which aligned lines are written.
        /// </summary>
        /// <returns>A terminal.</returns>
        public TerminalBase UnalignedTerminal { get; private set; }

        private int width;

        /// <inheritdoc/>
        public override int Width => width;

        private List<Action<TerminalBase>> commandBuffer;

        private int lineLength;

        /// <summary>
        /// Gets the length of the line that is currently in the buffer,
        /// measured in text elements.
        /// </summary>
        /// <returns>The current line length.</returns>
        public int BufferedLineLength => lineLength;

        private bool suppressNextLinePadding;

        /// <summary>
        /// Suppresses the padding for a single line.
        /// </summary>
        public void SuppressPadding()
        {
            suppressNextLinePadding = true;
        }

        /// <inheritdoc/>
        public override bool CanRender(string text)
        {
            return UnalignedTerminal.CanRender(text);
        }

        /// <inheritdoc/>
        public override void Write(string text)
        {
            Write(new StringInfo(text));
        }

        private void Write(StringInfo text)
        {
            int textLength = text.LengthInTextElements;
            if (textLength == 0)
            {
                return;
            }
            else if (lineLength + textLength <= width)
            {
                lineLength += textLength;
                commandBuffer.Add(new TerminalWriteCommand(text.String).Run);
            }
            else
            {
                var wrapped = WrapLine(text, Wrapping, lineLength, width);
                lineLength += LengthInTextElements(wrapped.Item1);
                commandBuffer.Add(new TerminalWriteCommand(wrapped.Item1).Run);
                WriteLine();
                Write(wrapped.Item2);
            }
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
            if (lineLength > 0 && !suppressNextLinePadding)
            {
                // Write padding.
                int padding = GetLeftPaddingSize(
                    Alignment, lineLength, width);
                for (int i = 0; i < padding; i++)
                {
                    UnalignedTerminal.Write(' ');
                }
                UnalignedTerminal.Write(LeftPadding);
            }
            suppressNextLinePadding = false;

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

        private static Tuple<string, string> WrapLine(
            StringInfo lineEnd,
            WrappingStrategy wrapping,
            int printedLineLength,
            int width)
        {
            // Make sure that the width >= 1 to prevent line-wrapping
            // logic from recursing forever.
            width = Math.Max(1, width);
            int charsLeftOnThisLine = width - printedLineLength;

            if (wrapping == WrappingStrategy.Word)
            {
                // Loop through the line and try to find the word that
                // straddles the boundary between the two lines. We want
                // to move that word onto the next line.
                var str = lineEnd.String;
                int wordStartIndex = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (char.IsWhiteSpace(str, i))
                    {
                        var substrInfo = new StringInfo(str.Substring(0, i));
                        int substrLength = substrInfo.LengthInTextElements;
                        if (substrLength > charsLeftOnThisLine)
                        {
                            // We found the first word that we'd like to
                            // put on the next line.
                            var firstNextLineWord = str.Substring(
                                wordStartIndex, i - wordStartIndex);

                            var firstNextLineWordLength = LengthInTextElements(
                                firstNextLineWord);

                            if (firstNextLineWordLength > width)
                            {
                                // The word is too big to put on the
                                // next line. Revert to per-character
                                // wrapping.
                                return SplitAtAndTrim(
                                    lineEnd, charsLeftOnThisLine);
                            }
                            else
                            {
                                // Move the word onto the next line.
                                return SplitAtAndTrim(
                                    lineEnd, wordStartIndex);
                            }
                        }
                        else
                        {
                            // We found a word, but it's not the one we're
                            // looking for. Record the character that trails
                            // the whitespace as the start of the next word.
                            wordStartIndex = i + 1;
                        }
                    }
                }

                if (wordStartIndex == 0)
                {
                    // Seems like we really can't split this thing.
                    // Use per-character wrapping as a fallback.
                    return SplitAt(lineEnd, charsLeftOnThisLine);
                }
                else
                {
                    // Split at the start of a word that isn't delimited
                    // by trailing whitespace.
                    return SplitAtAndTrim(lineEnd, wordStartIndex);
                }
            }

            // Per-character wrapping is easy: just print exactly the
            // number of characters left on the line.
            return SplitAt(lineEnd, charsLeftOnThisLine);
        }

        private static Tuple<string, string> SplitAt(
            StringInfo lineEnd,
            int offset)
        {
            return new Tuple<string, string>(
                lineEnd.SubstringByTextElements(0, offset),
                lineEnd.SubstringByTextElements(offset));
        }

        private static Tuple<string, string> SplitAtAndTrim(
            StringInfo lineEnd,
            int offset)
        {
            return new Tuple<string, string>(
                lineEnd.SubstringByTextElements(0, offset).TrimEnd(),
                lineEnd.SubstringByTextElements(offset).TrimStart());
        }

        private static int LengthInTextElements(string str)
        {
            return new StringInfo(str).LengthInTextElements;
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

        /// <summary>
        /// Creates an aligned terminal that aligns contents
        /// before printing them to another terminal.
        /// </summary>
        /// <param name="terminal">
        /// A terminal to print aligned contents to.
        /// </param>
        /// <param name="alignment">
        /// The alignment of the contents to print.
        /// </param>
        /// <returns>An aligned terminal.</returns>
        public static AlignedTerminal Align(
            TerminalBase terminal, Alignment alignment)
        {
            if (terminal is AlignedTerminal)
            {
                var alignedTerm = (AlignedTerminal)terminal;
                return new AlignedTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignment,
                    alignedTerm.Wrapping,
                    alignedTerm.LeftPadding,
                    alignedTerm.width);
            }
            else
            {
                return new AlignedTerminal(
                    terminal,
                    alignment,
                    WrappingStrategy.Character,
                    "",
                    terminal.Width);
            }
        }

        /// <summary>
        /// Creates an aligned terminal that wraps contents
        /// before printing them to another terminal.
        /// </summary>
        /// <param name="terminal">
        /// A terminal to print aligned contents to.
        /// </param>
        /// <param name="wrapping">
        /// The wrapping strategy to use.
        /// </param>
        /// <returns>A wrapping terminal.</returns>
        public static AlignedTerminal Wrap(
            TerminalBase terminal, WrappingStrategy wrapping)
        {
            if (terminal is AlignedTerminal)
            {
                var alignedTerm = (AlignedTerminal)terminal;
                return new AlignedTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignedTerm.Alignment,
                    wrapping,
                    alignedTerm.LeftPadding,
                    alignedTerm.width);
            }
            else
            {
                return new AlignedTerminal(
                    terminal,
                    Alignment.Left,
                    wrapping,
                    "",
                    terminal.Width);
            }
        }

        /// <summary>
        /// Creates an aligned terminal that inserts a horizontal
        /// margin.
        /// </summary>
        /// <param name="terminal">
        /// A terminal to print aligned contents to.
        /// </param>
        /// <param name="leftMargin">
        /// The size of the left margin.
        /// </param>
        /// <param name="rightMargin">
        /// The size of the right margin.
        /// </param>
        /// <returns>A margin-inserting terminal.</returns>
        public static AlignedTerminal AddHorizontalMargin(
            TerminalBase terminal, int leftMargin, int rightMargin)
        {
            var leftPadding = new StringBuilder()
                .Append(' ', leftMargin)
                .ToString();

            if (terminal is AlignedTerminal)
            {
                var alignedTerm = (AlignedTerminal)terminal;
                return new AlignedTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignedTerm.Alignment,
                    alignedTerm.Wrapping,
                    alignedTerm.LeftPadding + leftPadding,
                    Math.Max(1, alignedTerm.width - leftMargin - rightMargin));
            }
            else
            {
                return new AlignedTerminal(
                    terminal,
                    Alignment.Left,
                    WrappingStrategy.Character,
                    leftPadding,
                    Math.Max(1, terminal.Width - leftMargin - rightMargin));
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