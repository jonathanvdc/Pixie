using System;
using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Terminal.Layout
{
    internal abstract class InlineLayout
    { }

    internal sealed class EmptyInline : InlineLayout
    {
        public static readonly EmptyInline Instance = new EmptyInline();
        private EmptyInline() { }
    }

    internal sealed class TextRun : InlineLayout
    {
        public TextRun(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }

    internal sealed class InlineConcat : InlineLayout
    {
        public InlineConcat(IReadOnlyList<InlineLayout> children)
        {
            Children = children;
        }

        public IReadOnlyList<InlineLayout> Children { get; private set; }
    }

    internal sealed class StyledInline : InlineLayout
    {
        public StyledInline(InlineLayout body, Color foregroundColor, Color backgroundColor, TextDecoration decoration)
        {
            Body = body;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
            Decoration = decoration;
        }

        public InlineLayout Body { get; private set; }
        public Color ForegroundColor { get; private set; }
        public Color BackgroundColor { get; private set; }
        public TextDecoration Decoration { get; private set; }
    }

    internal sealed class AlternativeInline : InlineLayout
    {
        public AlternativeInline(string preferred, InlineLayout fallback)
        {
            Preferred = preferred;
            Fallback = fallback;
        }

        public string Preferred { get; private set; }
        public InlineLayout Fallback { get; private set; }
    }

    internal sealed class HardLine : InlineLayout
    {
        public static readonly HardLine Instance = new HardLine();
        private HardLine() { }
    }

    internal struct Margins
    {
        public Margins(int before, int after)
        {
            Before = before;
            After = after;
        }

        public int Before { get; private set; }
        public int After { get; private set; }

        public static readonly Margins None = new Margins(0, 0);
        public static readonly Margins Paragraph = new Margins(0, 1);
    }

    internal abstract class BlockLayout
    {
        protected BlockLayout(Margins margins)
        {
            Margins = margins;
        }

        public Margins Margins { get; private set; }
    }

    internal sealed class FlowBlock : BlockLayout
    {
        public FlowBlock(InlineLayout contents, Margins margins)
            : base(margins)
        {
            Contents = contents;
        }

        public InlineLayout Contents { get; private set; }
    }

    internal sealed class BlockStack : BlockLayout
    {
        public BlockStack(IReadOnlyList<BlockLayout> children)
            : base(Margins.None)
        {
            Children = children;
        }

        public IReadOnlyList<BlockLayout> Children { get; private set; }
    }

    internal sealed class LayoutBox : BlockLayout
    {
        public LayoutBox(BlockLayout contents, WrappingStrategy wrapping, Alignment alignment, int leftMargin, int rightMargin)
            : base(Margins.None)
        {
            Contents = contents;
            Wrapping = wrapping;
            Alignment = alignment;
            LeftMargin = leftMargin;
            RightMargin = rightMargin;
        }

        public BlockLayout Contents { get; private set; }
        public WrappingStrategy Wrapping { get; private set; }
        public Alignment Alignment { get; private set; }
        public int LeftMargin { get; private set; }
        public int RightMargin { get; private set; }
    }

    internal sealed class PrefixLayout : BlockLayout
    {
        public PrefixLayout(InlineLayout prefix, BlockLayout contents)
            : base(Margins.None)
        {
            Prefix = prefix;
            Contents = contents;
        }

        public InlineLayout Prefix { get; private set; }
        public BlockLayout Contents { get; private set; }
    }

    internal sealed class FixedBlock : BlockLayout
    {
        public FixedBlock(Action<TerminalBase> render, Margins margins)
            : base(margins)
        {
            Render = render;
        }

        public Action<TerminalBase> Render { get; private set; }
    }
}
