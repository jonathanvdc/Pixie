using Pixie.Markup;
using Pixie.Terminal.Devices;
using System;

namespace Pixie.Terminal.Layout
{
    internal sealed class LayoutRenderer
    {
        public void Render(BlockLayout block, TerminalBase terminal)
        {
            RenderBlock(block, terminal);
            terminal.FinishOutput();
        }

        private void RenderBlock(BlockLayout block, TerminalBase terminal)
        {
            switch (block)
            {
                case BlockStack stack:
                    RenderStack(stack, terminal);
                    break;
                case FlowBlock flow:
                    RenderInline(flow.Contents, terminal);
                    break;
                case LayoutBox box:
                    RenderLayoutBox(box, terminal);
                    break;
                case PrefixLayout prefix:
                    RenderPrefix(prefix, terminal);
                    break;
                case FixedBlock fixedBlock:
                    fixedBlock.Render(terminal);
                    break;
                default:
                    throw new NotSupportedException(
                        $"Unsupported block layout type: {block.GetType().FullName}");
            }
        }

        private void RenderStack(BlockStack stack, TerminalBase terminal)
        {
            BlockLayout previous = null;
            for (int i = 0; i < stack.Children.Count; i++)
            {
                var child = stack.Children[i];
                if (previous != null)
                {
                    int gap = System.Math.Max(GetLastMarginAfter(previous), GetFirstMarginBefore(child));
                    terminal.WriteSeparator(gap + 1);
                }
                else if (terminal.HasWrittenContent)
                {
                    int gap = GetFirstMarginBefore(child);
                    if (gap > 0)
                    {
                        terminal.WriteSeparator(gap + 1);
                    }
                }

                RenderBlock(child, terminal);
                previous = child;
            }
        }

        private int GetFirstMarginBefore(BlockLayout block)
        {
            switch (block)
            {
                case BlockStack stack:
                    return stack.Children.Count == 0
                        ? 0
                        : GetFirstMarginBefore(stack.Children[0]);
                case LayoutBox box:
                    return GetFirstMarginBefore(box.Contents);
                case PrefixLayout prefix:
                    return GetFirstMarginBefore(prefix.Contents);
                default:
                    return block.Margins.Before;
            }
        }

        private int GetLastMarginAfter(BlockLayout block)
        {
            switch (block)
            {
                case BlockStack stack:
                    return stack.Children.Count == 0
                        ? 0
                        : GetLastMarginAfter(stack.Children[stack.Children.Count - 1]);
                case LayoutBox box:
                    return GetLastMarginAfter(box.Contents);
                case PrefixLayout prefix:
                    return GetLastMarginAfter(prefix.Contents);
                default:
                    return block.Margins.After;
            }
        }

        private void RenderLayoutBox(LayoutBox box, TerminalBase terminal)
        {
            var inner = LayoutTerminal.AddHorizontalMargin(
                terminal,
                box.LeftMargin,
                box.RightMargin);
            inner = LayoutTerminal.Wrap(inner, box.Wrapping);
            inner = LayoutTerminal.Align(inner, box.Alignment);
            RenderBlock(box.Contents, inner);
            inner.Flush();
        }

        private void RenderPrefix(PrefixLayout prefix, TerminalBase terminal)
        {
            RenderInline(prefix.Prefix, terminal);
            int prefixLength = Measure(prefix.Prefix, terminal);
            if (terminal is LayoutTerminal layoutTerminal)
            {
                layoutTerminal.Flush();
            }

            var inner = LayoutTerminal.AddHorizontalMargin(terminal, prefixLength, 0);
            inner.SuppressPadding();
            RenderBlock(prefix.Contents, inner);
            inner.Flush();
        }

        private int Measure(InlineLayout inline, TerminalBase terminal)
        {
            switch (inline)
            {
                case EmptyInline:
                    return 0;
                case TextRun textRun:
                    return textRun.Text.Length;
                case InlineConcat concat:
                    int result = 0;
                    for (int i = 0; i < concat.Children.Count; i++)
                    {
                        result += Measure(concat.Children[i], terminal);
                    }
                    return result;
                case StyledInline styled:
                    return Measure(styled.Body, terminal);
                case AlternativeInline alternative:
                    return terminal.CanRender(alternative.Preferred)
                        ? alternative.Preferred.Length
                        : Measure(alternative.Fallback, terminal);
                default:
                    throw new NotSupportedException(
                        $"Unsupported inline layout type: {inline.GetType().FullName}");
            }
        }

        private void RenderInline(InlineLayout inline, TerminalBase terminal)
        {
            switch (inline)
            {
                case EmptyInline:
                    return;
                case TextRun textRun:
                    terminal.Write(textRun.Text);
                    return;
                case InlineConcat concat:
                    for (int i = 0; i < concat.Children.Count; i++)
                    {
                        RenderInline(concat.Children[i], terminal);
                    }
                    return;
                case StyledInline styled:
                    RenderStyled(styled, terminal);
                    return;
                case AlternativeInline alternative:
                    if (terminal.CanRender(alternative.Preferred))
                    {
                        terminal.Write(alternative.Preferred);
                    }
                    else
                    {
                        RenderInline(alternative.Fallback, terminal);
                    }
                    return;
                case HardLine:
                    terminal.WriteLine();
                    return;
                default:
                    throw new NotSupportedException(
                        $"Unsupported inline layout type: {inline.GetType().FullName}");
            }
        }

        private void RenderStyled(StyledInline styled, TerminalBase terminal)
        {
            int pushed = 0;
            if (styled.ForegroundColor.Alpha != 0.0)
            {
                terminal.Style.PushForegroundColor(styled.ForegroundColor);
                pushed++;
            }
            if (styled.BackgroundColor.Alpha != 0.0)
            {
                terminal.Style.PushBackgroundColor(styled.BackgroundColor);
                pushed++;
            }
            if (styled.Decoration != TextDecoration.None)
            {
                terminal.Style.PushDecoration(styled.Decoration, DecorationSpan.UnifyDecorations);
                pushed++;
            }
            try
            {
                RenderInline(styled.Body, terminal);
            }
            finally
            {
                for (int i = 0; i < pushed; i++)
                {
                    terminal.Style.PopStyle();
                }
            }
        }
    }
}
