using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// An enumeration of possible highlighted source span types.
    /// </summary>
    public enum HighlightedSourceSpanKind
    {
        /// <summary>
        /// Non-highlighted source code.
        /// </summary>
        Source,

        /// <summary>
        /// Highlighted source code with focus.
        /// </summary>
        Focus,

        /// <summary>
        /// Highlighted source code without focus.
        /// </summary>
        Highlight
    }

    /// <summary>
    /// Describes a highlighted span of source code.
    /// </summary>
    public struct HighlightedSourceSpan
    {
        /// <summary>
        /// Creates a highlighted source span from a kind and a
        /// string of text.
        /// </summary>
        /// <param name="kind">The source span's kind.</param>
        /// <param name="text">The source span's text.</param>
        public HighlightedSourceSpan(
            HighlightedSourceSpanKind kind,
            string text)
        {
            this = default(HighlightedSourceSpan);
            this.Kind = kind;
            this.Text = text;
        }

        /// <summary>
        /// Gets this source span's kind.
        /// </summary>
        /// <returns></returns>
        public HighlightedSourceSpanKind Kind { get; private set; }

        /// <summary>
        /// Gets this span of source code as text.
        /// </summary>
        /// <returns>The source span's text.</returns>
        public string Text { get; private set; }
    }

    /// <summary>
    /// A renderer for highlighted source code.
    /// </summary>
    public class HighlightedSourceRenderer : NodeRenderer
    {
        /// <summary>
        /// Create a highlighted source renderer that highlights source
        /// using a particular color.
        /// </summary>
        /// <param name="highlightColor">The highlight color.</param>
        /// <param name="contextLineCount">
        /// The number of lines that are printed for context
        /// below and above the focus line.
        /// </param>
        public HighlightedSourceRenderer(
            Color highlightColor,
            int contextLineCount)
        {
            this.HighlightColor = highlightColor;
            this.ContextLineCount = contextLineCount;
        }

        /// <summary>
        /// Gets the color with which source code is highlighted.
        /// </summary>
        /// <returns>The highlight color.</returns>
        public Color HighlightColor { get; private set; }

        /// <summary>
        /// Gets the number of lines that are printed for context
        /// below and above the focus line.
        /// </summary>
        /// <returns>The number of context lines.</returns>
        public int ContextLineCount { get; private set; }

        /// <inheritdoc/>
        public override bool CanRender(MarkupNode node)
        {
            return node is HighlightedSource;
        }

        /// <inheritdoc/>
        public override void Render(MarkupNode node, RenderState state)
        {
            var src = (HighlightedSource)node;

            var highlightRegion = src.HighlightRegion;
            var focusRegion = src.FocusRegion;
            var document = focusRegion.Document;

            // The idea is to visualize the first line of the focus region,
            // plus a number of lines of context.
            int focusLine = document.GetGridPosition(focusRegion.StartOffset).LineIndex;

            int firstLineNumber = -1;
            var lines = new List<IReadOnlyList<HighlightedSourceSpan>>();
            for (int i = -ContextLineCount; i <= ContextLineCount; i++)
            {
                var lineSpans = LineToSpans(focusLine + i, highlightRegion, focusRegion);
                if (lineSpans != null)
                {
                    if (firstLineNumber < 0)
                    {
                        firstLineNumber = focusLine + i;
                    }
                    lines.Add(lineSpans);
                }
            }

            var compressedLines = CompressLeadingWhitespace(lines);
            var maxLineWidth = GetLineWidth(
                firstLineNumber + compressedLines.Count, state);
            for (int i = 0; i < compressedLines.Count; i++)
            {
                RenderLine(
                    WrapSpans(compressedLines[i], maxLineWidth),
                    firstLineNumber + i,
                    firstLineNumber + compressedLines.Count,
                    state);
            }
        }

        /// <summary>
        /// Gets the number of characters a source line can be wide.
        /// </summary>
        /// <param name="greatestLineIndex">The greatest line index to render.</param>
        /// <param name="state">A render state.</param>
        /// <returns>The line width.</returns>
        protected virtual int GetLineWidth(int greatestLineIndex, RenderState state)
        {
            return state.Terminal.Width
                - GetLineContinuatorPrefix(greatestLineIndex, greatestLineIndex, state).Length // Left padding
                - 4; // Right padding
        }

        /// <summary>
        /// Renders a single line of code that has been line-wrapped.
        /// </summary>
        /// <param name="wrappedSpans">
        /// A line-wrapped list of highlighted spans.
        /// </param>
        /// <param name="lineIndex">The line's zero-based index.</param>
        /// <param name="greatestLineIndex">
        /// The greatest zero-based line index that will be rendered.
        /// </param>
        /// <param name="state">The render state.</param>
        protected virtual void RenderLine(
            IReadOnlyList<IReadOnlyList<HighlightedSourceSpan>> wrappedSpans,
            int lineIndex,
            int greatestLineIndex,
            RenderState state)
        {
            state.Terminal.Write(
                GetLineNumberPrefix(lineIndex, greatestLineIndex, state));

            var continuatorPrefix = GetLineContinuatorPrefix(
                lineIndex, greatestLineIndex, state);

            var newTerm = new LayoutTerminal(
                state.Terminal,
                Alignment.Left,
                WrappingStrategy.Character,
                continuatorPrefix,
                state.Terminal.Width);

            newTerm.SuppressPadding();

            var newState = state.WithTerminal(newTerm);

            var wrappedSpanCount = wrappedSpans.Count;
            for (int i = 0; i < wrappedSpanCount; i++)
            {
                foreach (var span in wrappedSpans[i])
                {
                    RenderSpanText(span, newState);
                }
                newTerm.WriteLine();

                if (IsHighlighted(wrappedSpans[i]))
                {
                    foreach (var span in wrappedSpans[i])
                    {
                        RenderSpanSquiggle(span, newState);
                    }
                    newTerm.WriteLine();
                }
            }

            newTerm.Flush();
        }

        private const string leftWhitespace = "  ";

        /// <summary>
        /// Gets the line number prefix that. is prepended to
        /// each new line.
        /// </summary>
        /// <param name="lineIndex">The index of the line.</param>
        /// <param name="greatestLineIndex">
        /// The greatest line index that will be rendered.
        /// </param>
        /// <param name="state">
        /// A render state.
        /// </param>
        /// <returns>A line number prefix.</returns>
        protected virtual string GetLineNumberPrefix(
            int lineIndex,
            int greatestLineIndex,
            RenderState state)
        {
            var numString = (lineIndex + 1).ToString();
            var maxString = (greatestLineIndex + 1).ToString();
            var sb = new StringBuilder();
            sb.Append(leftWhitespace);
            sb.Append(' ', maxString.Length - numString.Length);
            sb.Append(numString);
            sb.Append(GetSeparatorBar(state));
            return sb.ToString();
        }

        /// <summary>
        /// Gets the line continuator prefix that. is prepended to
        /// the start of each wrapped line.
        /// </summary>
        /// <param name="lineIndex">The index of the line.</param>
        /// <param name="greatestLineIndex">
        /// The greatest line index that will be rendered.
        /// </param>
        /// <param name="state">
        /// A render state.
        /// </param>
        /// <returns>A line continuator prefix.</returns>
        protected virtual string GetLineContinuatorPrefix(
            int lineIndex, int greatestLineIndex, RenderState state)
        {
            var maxString = (greatestLineIndex + 1).ToString();
            var sb = new StringBuilder();
            sb.Append(leftWhitespace);
            sb.Append(' ', maxString.Length);
            sb.Append(GetSeparatorBar(state));
            return sb.ToString();
        }

        /// <summary>
        /// Gets a string that contains a bar with whitespace on both sides,
        /// useful for delimiting line numbers and code.
        /// </summary>
        /// <param name="state">A render state.</param>
        /// <returns>A string of characters.</returns>
        protected string GetSeparatorBar(RenderState state)
        {
            var sb = new StringBuilder();
            sb.Append(' ');
            string unicodeBar = "\u2502";
            sb.Append(state.Terminal.CanRender(unicodeBar) ? unicodeBar : "|");
            sb.Append(' ');
            return sb.ToString();
        }

        /// <summary>
        /// Renders a span's text.
        /// </summary>
        /// <param name="span">A span whose text is to be rendered.</param>
        /// <param name="state">A render state.</param>
        protected virtual void RenderSpanText(
            HighlightedSourceSpan span,
            RenderState state)
        {
            if (span.Kind == HighlightedSourceSpanKind.Focus)
            {
                state.Terminal.Style.PushForegroundColor(HighlightColor);
                state.Terminal.Style.PushDecoration(
                    TextDecoration.Bold, DecorationSpan.UnifyDecorations);
                try
                {
                    state.Terminal.Write(span.Text);
                }
                finally
                {
                    state.Terminal.Style.PopStyle();
                    state.Terminal.Style.PopStyle();
                }
            }
            else
            {
                state.Terminal.Write(span.Text);
            }
        }

        /// <summary>
        /// Renders the squiggle under a span.
        /// </summary>
        /// <param name="span">A span for which a squiggle is to be rendered.</param>
        /// <param name="state">A render state.</param>
        protected virtual void RenderSpanSquiggle(
            HighlightedSourceSpan span,
            RenderState state)
        {
            var spanLength = new StringInfo(span.Text).LengthInTextElements;
            char caret = '^';
            char squiggle = '~';
            if (span.Kind == HighlightedSourceSpanKind.Highlight)
            {
                state.Terminal.Style.PushForegroundColor(HighlightColor);
                try
                {
                    for (int i = 0; i < spanLength; i++)
                    {
                        state.Terminal.Write(squiggle);
                    }
                }
                finally
                {
                    state.Terminal.Style.PopStyle();
                }
            }
            else if (span.Kind == HighlightedSourceSpanKind.Focus)
            {
                state.Terminal.Style.PushForegroundColor(HighlightColor);
                state.Terminal.Style.PushDecoration(
                    TextDecoration.Bold, DecorationSpan.UnifyDecorations);
                try
                {
                    for (int i = 0; i < spanLength; i++)
                    {
                        state.Terminal.Write(i == 0 ? caret : squiggle);
                    }
                }
                finally
                {
                    state.Terminal.Style.PopStyle();
                    state.Terminal.Style.PopStyle();
                }
            }
            else
            {
                for (int i = 0; i < spanLength; i++)
                {
                    state.Terminal.Write(' ');
                }
            }
        }

        /// <summary>
        /// Identifies and compresses leading whitespace in a list
        /// of lines, where each line is encoded as a highlighted span.
        /// </summary>
        /// <param name="lines">A list of lines.</param>
        /// <returns>A new list of lines.</returns>
        protected virtual IReadOnlyList<IReadOnlyList<HighlightedSourceSpan>> CompressLeadingWhitespace(
            IReadOnlyList<IReadOnlyList<HighlightedSourceSpan>> lines)
        {
            // TODO: implement this
            return lines;
        }

        /// <summary>
        /// Line-wraps a list of highlighted source spans.
        /// </summary>
        /// <param name="spans">The spans to line-wrap.</param>
        /// <param name="lineLength">The maximum length of a line.</param>
        /// <returns>A list of lines, where each line is encoded as a list of spans.</returns>
        protected virtual IReadOnlyList<IReadOnlyList<HighlightedSourceSpan>> WrapSpans(
            IReadOnlyList<HighlightedSourceSpan> spans,
            int lineLength)
        {
            var allLines = new List<IReadOnlyList<HighlightedSourceSpan>>();
            var currentLine = new List<HighlightedSourceSpan>();

            var spanCount = spans.Count;
            int length = 0;
            for (int i = 0; i < spanCount; i++)
            {
                AppendSpanToLine(spans[i], ref currentLine, ref length, allLines, lineLength);
            }

            if (currentLine.Count != 0)
            {
                allLines.Add(currentLine);
            }

            return allLines;
        }

        private static void AppendSpanToLine(
            HighlightedSourceSpan span,
            ref List<HighlightedSourceSpan> line,
            ref int lineLength,
            List<IReadOnlyList<HighlightedSourceSpan>> allLines,
            int maxLineLength)
        {
            var spanInfo = new StringInfo(span.Text);
            if (spanInfo.LengthInTextElements == 0)
            {
                // Do nothing.
            }
            else if (lineLength + spanInfo.LengthInTextElements <= maxLineLength)
            {
                // Easy case: append entire span to line.
                line.Add(span);
                lineLength += spanInfo.LengthInTextElements;
            }
            else
            {
                // Slightly more complicated case: split the span.
                var remainingElements = maxLineLength - lineLength;
                var first = spanInfo.SubstringByTextElements(0, remainingElements);
                var second = spanInfo.SubstringByTextElements(remainingElements);

                // Append the first part.
                line.Add(new HighlightedSourceSpan(span.Kind, first));

                // Start a new line.
                allLines.Add(line);
                line = new List<HighlightedSourceSpan>();
                lineLength = 0;

                // Append the second part.
                AppendSpanToLine(
                    new HighlightedSourceSpan(span.Kind, second),
                    ref line,
                    ref lineLength,
                    allLines,
                    maxLineLength);
            }
        }

        private static HighlightedSourceSpanKind GetCharacterKind(
            int offset,
            SourceRegion highlightRegion,
            SourceRegion focusRegion)
        {
            if (focusRegion.Contains(offset))
                return HighlightedSourceSpanKind.Focus;
            else if (highlightRegion.Contains(offset))
                return HighlightedSourceSpanKind.Highlight;
            else
                return HighlightedSourceSpanKind.Source;
        }

        /// <summary>
        /// Chunks a line into a list of highlighted spans.
        /// </summary>
        /// <param name="lineText">The line's text.</param>
        /// <param name="lineStartOffset">
        /// The offset of the first character in the line.
        /// </param>
        /// <param name="highlightRegion">
        /// The highlighted region.
        /// </param>
        /// <param name="focusRegion">
        /// The focus region.
        /// </param>
        /// <returns>A list of highlighted spans.</returns>
        private static IReadOnlyList<HighlightedSourceSpan> LineToSpans(
            string lineText,
            int lineStartOffset,
            SourceRegion highlightRegion,
            SourceRegion focusRegion)
        {
            var spans = new List<HighlightedSourceSpan>();

            var kind = HighlightedSourceSpanKind.Source;
            int spanStart = 0;
            for (int i = 0; i < lineText.Length; i++)
            {
                var charKind = GetCharacterKind(
                    lineStartOffset + i, highlightRegion, focusRegion);
                
                if (charKind != kind)
                {
                    if (spanStart != i)
                    {
                        spans.Add(
                            new HighlightedSourceSpan(
                                kind,
                                lineText
                                    .Substring(spanStart, i - spanStart)
                                    .Replace("\t", "    ")));
                    }
                    kind = charKind;
                    spanStart = i;
                }
            }

            if (spanStart != lineText.Length)
            {
                spans.Add(
                    new HighlightedSourceSpan(
                        kind,
                        lineText
                            .Substring(spanStart)
                            .TrimEnd()
                            .Replace("\t", "    ")));
            }
            
            return spans;
        }

        /// <summary>
        /// Chunks a line into a list of highlighted spans.
        /// </summary>
        /// <param name="lineIndex">The line's zero-based index.</param>
        /// <param name="highlightRegion">
        /// The highlighted region.
        /// </param>
        /// <param name="focusRegion">
        /// The focus region.
        /// </param>
        /// <returns>A list of highlighted spans.</returns>
        private static IReadOnlyList<HighlightedSourceSpan> LineToSpans(
            int lineIndex,
            SourceRegion highlightRegion,
            SourceRegion focusRegion)
        {
            var document = focusRegion.Document;

            int lineStart = document.GetLineOffset(lineIndex);
            int lineEnd = document.GetLineOffset(lineIndex + 1);

            if (lineStart == lineEnd)
            {
                // Nothing to do here.
                return null;
            }

            var lineText = document.GetText(lineStart, lineEnd - lineStart).TrimEnd();

            return LineToSpans(lineText, lineStart, highlightRegion, focusRegion);
        }

        private static bool IsHighlighted(
            IReadOnlyList<HighlightedSourceSpan> spans)
        {
            return spans.Any<HighlightedSourceSpan>(IsHighlighted);
        }

        private static bool IsHighlighted(
            HighlightedSourceSpan span)
        {
            return span.Kind != HighlightedSourceSpanKind.Source;
        }
    }
}