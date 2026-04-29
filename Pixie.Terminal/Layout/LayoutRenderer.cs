using Pixie.Markup;
using Pixie.Terminal.Devices;

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
            if (block is BlockStack)
            {
                RenderStack((BlockStack)block, terminal);
            }
            else if (block is FlowBlock)
            {
                RenderInline(((FlowBlock)block).Contents, terminal);
            }
            else if (block is LayoutBox)
            {
                RenderLayoutBox((LayoutBox)block, terminal);
            }
            else if (block is PrefixLayout)
            {
                RenderPrefix((PrefixLayout)block, terminal);
            }
            else if (block is FixedBlock)
            {
                ((FixedBlock)block).Render(terminal);
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
            if (block is BlockStack)
            {
                var stack = (BlockStack)block;
                return stack.Children.Count == 0
                    ? 0
                    : GetFirstMarginBefore(stack.Children[0]);
            }
            if (block is LayoutBox)
            {
                return GetFirstMarginBefore(((LayoutBox)block).Contents);
            }
            if (block is PrefixLayout)
            {
                return GetFirstMarginBefore(((PrefixLayout)block).Contents);
            }
            return block.Margins.Before;
        }

        private int GetLastMarginAfter(BlockLayout block)
        {
            if (block is BlockStack)
            {
                var stack = (BlockStack)block;
                return stack.Children.Count == 0
                    ? 0
                    : GetLastMarginAfter(stack.Children[stack.Children.Count - 1]);
            }
            if (block is LayoutBox)
            {
                return GetLastMarginAfter(((LayoutBox)block).Contents);
            }
            if (block is PrefixLayout)
            {
                return GetLastMarginAfter(((PrefixLayout)block).Contents);
            }
            return block.Margins.After;
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
            if (terminal is LayoutTerminal)
            {
                ((LayoutTerminal)terminal).Flush();
            }

            var inner = LayoutTerminal.AddHorizontalMargin(terminal, prefixLength, 0);
            inner.SuppressPadding();
            RenderBlock(prefix.Contents, inner);
            inner.Flush();
        }

        private int Measure(InlineLayout inline, TerminalBase terminal)
        {
            if (inline is EmptyInline)
            {
                return 0;
            }
            if (inline is TextRun)
            {
                return ((TextRun)inline).Text.Length;
            }
            if (inline is InlineConcat)
            {
                var concat = (InlineConcat)inline;
                int result = 0;
                for (int i = 0; i < concat.Children.Count; i++)
                {
                    result += Measure(concat.Children[i], terminal);
                }
                return result;
            }
            if (inline is StyledInline)
            {
                return Measure(((StyledInline)inline).Body, terminal);
            }
            if (inline is AlternativeInline)
            {
                var alternative = (AlternativeInline)inline;
                return terminal.CanRender(alternative.Preferred)
                    ? alternative.Preferred.Length
                    : Measure(alternative.Fallback, terminal);
            }
            return 0;
        }

        private void RenderInline(InlineLayout inline, TerminalBase terminal)
        {
            if (inline is EmptyInline)
            {
                return;
            }
            if (inline is TextRun)
            {
                terminal.Write(((TextRun)inline).Text);
                return;
            }
            if (inline is InlineConcat)
            {
                var concat = (InlineConcat)inline;
                for (int i = 0; i < concat.Children.Count; i++)
                {
                    RenderInline(concat.Children[i], terminal);
                }
                return;
            }
            if (inline is StyledInline)
            {
                RenderStyled((StyledInline)inline, terminal);
                return;
            }
            if (inline is AlternativeInline)
            {
                var alternative = (AlternativeInline)inline;
                if (terminal.CanRender(alternative.Preferred))
                {
                    terminal.Write(alternative.Preferred);
                }
                else
                {
                    RenderInline(alternative.Fallback, terminal);
                }
                return;
            }
            if (inline is HardLine)
            {
                terminal.WriteLine();
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
