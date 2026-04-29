using System;
using System.Collections.Generic;
using Pixie.Code;
using Pixie.Markup;

namespace Pixie.Terminal.Layout
{
    internal static class HighlightedSourceFormatter
    {
        public static void Render(
            HighlightedSource source,
            TerminalBase terminal,
            int contextLineCount,
            Color highlightColor)
        {
            var resolvedFocus = source.FocusRegion.BoundingSpan.Resolve();
            var primaryFocus = resolvedFocus.PrimarySpan;
            var document = primaryFocus.Document;
            var highlight = ResolveRegionForDocument(source.HighlightedRegion, document);
            var focus = ResolveRegionForDocument(source.FocusRegion, document);
            if (highlight == null || focus == null)
            {
                return;
            }

            int focusLine = document.GetLineByOffset(primaryFocus.Start).Index;
            var lines = new List<SourceLine>();
            for (int i = -contextLineCount; i <= contextLineCount; i++)
            {
                SourceLine line;
                if (document.TryGetLine(focusLine + i, out line))
                {
                    lines.Add(line);
                }
            }

            if (lines.Count == 0)
            {
                return;
            }

            int greatestLine = lines[lines.Count - 1].Index;
            int width = (greatestLine + 1).ToString().Length;
            string separator = " " + (terminal.GetFirstRenderableString("\u2502", "|", "-") ?? "|") + " ";
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                terminal.Write("  ");
                terminal.Write((line.Index + 1).ToString().PadLeft(width));
                terminal.Write(separator);
                RenderSourceLine(line, highlight, focus, terminal, highlightColor);
                terminal.WriteLine();

                if (LineHasHighlight(line, highlight, focus))
                {
                    terminal.Write("  ");
                    terminal.Write(new string(' ', width));
                    terminal.Write(separator);
                    RenderSquiggleLine(line, highlight, focus, terminal, highlightColor);
                    terminal.WriteLine();
                }
            }
        }

        private static SourceRegion ResolveRegionForDocument(
            SourceRegion region,
            OriginalSourceDocument document)
        {
            if (ReferenceEquals(region.Document, document))
            {
                return region;
            }

            var resolved = region.BoundingSpan.Resolve();
            SourceRegion result = null;
            for (int i = 0; i < resolved.OriginSpans.Count; i++)
            {
                var span = resolved.OriginSpans[i];
                if (!ReferenceEquals(span.Document, document))
                {
                    continue;
                }

                var sourceSpan = new SourceSpan(span.Document, span.Start, span.Length);
                result = result == null
                    ? new SourceRegion(sourceSpan)
                    : result.Union(sourceSpan);
            }

            return result;
        }

        private static bool LineHasHighlight(
            SourceLine line,
            SourceRegion highlight,
            SourceRegion focus)
        {
            int visibleLength = line.Text.TrimEnd().Length;
            for (int i = visibleLength; i < line.Text.Length; i++)
            {
                int offset = line.Start + i;
                if (focus.Contains(offset) || highlight.Contains(offset))
                {
                    visibleLength = i + 1;
                }
            }
            for (int i = 0; i < visibleLength; i++)
            {
                int offset = line.Start + i;
                if (focus.Contains(offset) || highlight.Contains(offset))
                {
                    return true;
                }
            }
            return false;
        }

        private static void RenderSourceLine(
            SourceLine line,
            SourceRegion highlight,
            SourceRegion focus,
            TerminalBase terminal,
            Color highlightColor)
        {
            string text = line.Text.TrimEnd();
            for (int i = 0; i < text.Length; i++)
            {
                string cell = text[i] == '\t'
                    ? "    "
                    : text[i].ToString();
                if (focus.Contains(line.Start + i))
                {
                    terminal.Style.PushForegroundColor(highlightColor);
                    terminal.Style.PushDecoration(
                        TextDecoration.Bold,
                        DecorationSpan.UnifyDecorations);
                    try
                    {
                        terminal.Write(cell);
                    }
                    finally
                    {
                        terminal.Style.PopStyle();
                        terminal.Style.PopStyle();
                    }
                }
                else
                {
                    terminal.Write(cell);
                }
            }
        }

        private static void RenderSquiggleLine(
            SourceLine line,
            SourceRegion highlight,
            SourceRegion focus,
            TerminalBase terminal,
            Color highlightColor)
        {
            int visibleLength = line.Text.TrimEnd().Length;
            for (int i = visibleLength; i < line.Text.Length; i++)
            {
                int offset = line.Start + i;
                if (focus.Contains(offset) || highlight.Contains(offset))
                {
                    visibleLength = i + 1;
                }
            }

            bool hasFocus = false;
            int lastHighlight = -1;
            for (int i = 0; i < visibleLength; i++)
            {
                int offset = line.Start + i;
                if (focus.Contains(offset) || highlight.Contains(offset))
                {
                    lastHighlight = i;
                }
            }
            for (int i = 0; i < visibleLength; i++)
            {
                int offset = line.Start + i;
                if (focus.Contains(offset))
                {
                    terminal.Style.PushForegroundColor(highlightColor);
                    terminal.Style.PushDecoration(
                        TextDecoration.Bold,
                        DecorationSpan.UnifyDecorations);
                    try
                    {
                        terminal.Write(hasFocus ? '~' : '^');
                    }
                    finally
                    {
                        terminal.Style.PopStyle();
                        terminal.Style.PopStyle();
                    }
                    hasFocus = true;
                }
                else if (highlight.Contains(offset))
                {
                    terminal.Style.PushForegroundColor(highlightColor);
                    try
                    {
                        terminal.Write('~');
                    }
                    finally
                    {
                        terminal.Style.PopStyle();
                    }
                }
                else
                {
                    if (i > lastHighlight)
                    {
                        break;
                    }
                    terminal.Write(' ');
                }
            }
        }
    }
}
