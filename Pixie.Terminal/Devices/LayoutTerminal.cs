using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Pixie.Markup;
using Pixie.Terminal.Render;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that buffers lines, aligns them, word-wraps them
    /// and writes them to another terminal.
    /// </summary>
    public sealed class LayoutTerminal : TerminalBase
    {
        /// <summary>
        /// Creates a layout terminal from an unaligned terminal,
        /// an alignment, a wrapping strategy, a left padding string
        /// and a terminal width.
        /// </summary>
        /// <param name="unalignedTerminal">
        /// A terminal to write formatted output to.
        /// </param>
        /// <param name="alignment">
        /// The alignment for each line.
        /// </param>
        /// <param name="wrapping">
        /// The wrapping strategy to use.
        /// </param>
        /// <param name="leftPadding">
        /// A string that is appended to the left of each line.
        /// It serves as padding.
        /// </param>
        /// <param name="width">
        /// The width of the terminal: the number of characters that fit on a line.
        /// </param>
        public LayoutTerminal(
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
            this.commandBuffer = new List<Action<TerminalBase>>();
            this.style = new BufferingStyleManager(this.commandBuffer);
            Reset();
        }

        private StyleManager style;

        /// <inheritdoc/>
        public override StyleManager Style => style;

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
        /// Starts a layout box. Flushes any buffered output
        /// in the render state to the output terminal, creates
        /// a new render state based on this layout terminal and
        /// appends a line separator.
        /// </summary>
        /// <param name="state">
        /// The old render state to create a new box in.
        /// </param>
        /// <returns>A new render state for a layout box.</returns>
        public RenderState StartLayoutBox(RenderState state)
        {
            if (state.Terminal is LayoutTerminal)
            {
                ((LayoutTerminal)state.Terminal).Flush();
            }
            WriteSeparator(1);
            return state.WithTerminal(this);
        }

        /// <summary>
        /// Ends a layout box by appending a line separator.
        /// </summary>
        public void EndLayoutBox()
        {
            WriteSeparator(1);
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
            commandBuffer.Clear();
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
        public static LayoutTerminal Align(
            TerminalBase terminal, Alignment alignment)
        {
            if (terminal is LayoutTerminal)
            {
                var alignedTerm = (LayoutTerminal)terminal;
                return new LayoutTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignment,
                    alignedTerm.Wrapping,
                    alignedTerm.LeftPadding,
                    alignedTerm.width);
            }
            else
            {
                return new LayoutTerminal(
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
        public static LayoutTerminal Wrap(
            TerminalBase terminal, WrappingStrategy wrapping)
        {
            if (terminal is LayoutTerminal)
            {
                var alignedTerm = (LayoutTerminal)terminal;
                return new LayoutTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignedTerm.Alignment,
                    wrapping,
                    alignedTerm.LeftPadding,
                    alignedTerm.width);
            }
            else
            {
                return new LayoutTerminal(
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
        public static LayoutTerminal AddHorizontalMargin(
            TerminalBase terminal, int leftMargin, int rightMargin)
        {
            var leftPadding = new StringBuilder()
                .Append(' ', leftMargin)
                .ToString();

            if (terminal is LayoutTerminal)
            {
                var alignedTerm = (LayoutTerminal)terminal;
                return new LayoutTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignedTerm.Alignment,
                    alignedTerm.Wrapping,
                    alignedTerm.LeftPadding + leftPadding,
                    Math.Max(1, alignedTerm.width - leftMargin - rightMargin));
            }
            else
            {
                return new LayoutTerminal(
                    terminal,
                    Alignment.Left,
                    WrappingStrategy.Character,
                    leftPadding,
                    Math.Max(1, terminal.Width - leftMargin - rightMargin));
            }
        }

        /// <summary>
        /// Creates a layout terminal that inserts additional left padding.
        /// </summary>
        /// <param name="terminal">
        /// A terminal to print aligned contents to.
        /// </param>
        /// <param name="extraLeftPadding">
        /// Additional left padding to print at the start of each line.
        /// </param>
        /// <returns>A padding-inserting terminal.</returns>
        public static LayoutTerminal AddLeftPadding(
            TerminalBase terminal, string extraLeftPadding)
        {
            if (terminal is LayoutTerminal)
            {
                var alignedTerm = (LayoutTerminal)terminal;
                return new LayoutTerminal(
                    alignedTerm.UnalignedTerminal,
                    alignedTerm.Alignment,
                    alignedTerm.Wrapping,
                    alignedTerm.LeftPadding + extraLeftPadding,
                    Math.Max(1, alignedTerm.width - extraLeftPadding.Length));
            }
            else
            {
                return new LayoutTerminal(
                    terminal,
                    Alignment.Left,
                    WrappingStrategy.Character,
                    extraLeftPadding,
                    Math.Max(1, terminal.Width - extraLeftPadding.Length));
            }
        }
    }

    internal sealed class BufferingStyleManager : StyleManager
    {
        public BufferingStyleManager(
            List<Action<TerminalBase>> commandBuffer)
        {
            this.commandBuffer = commandBuffer;
        }

        private List<Action<TerminalBase>> commandBuffer;

        /// <inheritdoc/>
        public override void PushForegroundColor(Color color)
        {
            commandBuffer.Add(new ForegroundColorCommand(color).Run);
        }

        /// <inheritdoc/>
        public override void PushBackgroundColor(Color color)
        {
            commandBuffer.Add(new BackgroundColorCommand(color).Run);
        }

        /// <inheritdoc/>
        public override void PushDecoration(
            TextDecoration decoration,
            Func<TextDecoration, TextDecoration, TextDecoration> updateDecoration)
        {
            commandBuffer.Add(new DecorationCommand(decoration, updateDecoration).Run);
        }

        /// <inheritdoc/>
        public override void PopStyle()
        {
            commandBuffer.Add(PopStyle);
        }

        private static void PopStyle(TerminalBase terminal)
        {
            terminal.Style.PopStyle();
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

    internal sealed class ForegroundColorCommand
    {
        public ForegroundColorCommand(Color color)
        {
            this.Color = color;
        }

        public Color Color { get; private set; }

        public void Run(TerminalBase terminal)
        {
            terminal.Style.PushForegroundColor(Color);
        }
    }

    internal sealed class BackgroundColorCommand
    {
        public BackgroundColorCommand(Color color)
        {
            this.Color = color;
        }

        public Color Color { get; private set; }

        public void Run(TerminalBase terminal)
        {
            terminal.Style.PushBackgroundColor(Color);
        }
    }

    internal sealed class DecorationCommand
    {
        public DecorationCommand(
            TextDecoration decoration,
            Func<TextDecoration, TextDecoration, TextDecoration> updateDecoration)
        {
            this.Decoration = decoration;
            this.UpdateDecoration = updateDecoration;
        }

        public TextDecoration Decoration { get; private set; }

       public  Func<TextDecoration, TextDecoration, TextDecoration> UpdateDecoration { get; private set; }

        public void Run(TerminalBase terminal)
        {
            terminal.Style.PushDecoration(Decoration, UpdateDecoration);
        }
    }
}