using System;
using System.Text;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Terminal.Devices;

namespace Pixie.Terminal.Render
{
    /// <summary>
    /// A renderer for highlighted source code.
    /// </summary>
    public sealed class HighlightedSourceRenderer : NodeRenderer
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

            for (int i = -ContextLineCount; i <= ContextLineCount; i++)
            {
                RenderLine(focusLine + i, highlightRegion, focusRegion, state);
            }
        }

        private void RenderLine(
            int line,
            SourceRegion highlightRegion,
            SourceRegion focusRegion,
            RenderState state)
        {
            var document = focusRegion.Document;

            int lineStart = document.GetLineOffset(line);
            int lineEnd = document.GetLineOffset(line + 1);

            if (lineStart == lineEnd)
            {
                // Nothing to do here.
                return;
            }

            var lineText = document.GetText(lineStart, lineEnd - lineStart).TrimEnd();

            RenderLine(lineText, lineStart, highlightRegion, focusRegion, state);
        }

        private void RenderLine(
            string lineText,
            int lineStartOffset,
            SourceRegion highlightRegion,
            SourceRegion focusRegion,
            RenderState state)
        {
            var caret = '^';
            var squiggle = '~';

            int offset = 0;
            while (offset < lineText.Length)
            {
                var substr = lineText
                    .Substring(
                        offset,
                        Math.Min(lineText.Length - offset, state.Terminal.Width))
                    .ToCharArray();

                var caretSquiggleLine = new StringBuilder();
                for (int i = 0; i < substr.Length; i++)
                {
                    if (focusRegion.Contains(lineStartOffset + offset))
                    {
                        state.Terminal.Style.PushForegroundColor(HighlightColor);
                        state.Terminal.Style.PushDecoration(
                            TextDecoration.Bold, DecorationSpan.UnifyDecorations);
                        try
                        {
                            state.Terminal.Write(substr[i]);
                        }
                        finally
                        {
                            state.Terminal.Style.PopStyle();
                            state.Terminal.Style.PopStyle();
                        }
                        caretSquiggleLine.Append(caret);
                    }
                    else if (highlightRegion.Contains(lineStartOffset + offset))
                    {
                        state.Terminal.Write(substr[i]);
                        caretSquiggleLine.Append(squiggle);
                    }
                    else
                    {
                        state.Terminal.Write(substr[i]);
                        caretSquiggleLine.Append(" ");
                    }
                    offset++;
                }
                state.Terminal.WriteLine();

                if (!string.IsNullOrWhiteSpace(caretSquiggleLine.ToString()))
                {
                    state.Terminal.Style.PushForegroundColor(HighlightColor);
                    try
                    {
                        for (int i = 0; i < caretSquiggleLine.Length; i++)
                        {
                            if (caretSquiggleLine[i] == caret)
                            {
                                state.Terminal.Style.PushDecoration(
                                    TextDecoration.Bold, DecorationSpan.UnifyDecorations);
                                try
                                {
                                    state.Terminal.Write(caret);
                                }
                                finally
                                {
                                    state.Terminal.Style.PopStyle();
                                }
                            }
                            else
                            {
                                state.Terminal.Write(caretSquiggleLine[i]);
                            }
                        }
                        state.Terminal.WriteLine();
                    }
                    finally
                    {
                        state.Terminal.Style.PopStyle();
                    }
                }
            }
        }
    }
}