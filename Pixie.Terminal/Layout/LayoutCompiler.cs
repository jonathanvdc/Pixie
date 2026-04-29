using System;
using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Terminal.Layout
{
    internal sealed class LayoutCompiler
    {
        public LayoutCompiler()
            : this(Colors.Green)
        { }

        private LayoutCompiler(Color highlightColor)
        {
            this.highlightColor = highlightColor;
        }

        private readonly Color highlightColor;

        public InlineLayout CompileInline(Inline node)
        {
            if (node == null || Text.IsEmpty(node))
            {
                return EmptyInline.Instance;
            }

            if (node is Text)
            {
                return new TextRun(((Text)node).Contents);
            }
            if (node is Sequence)
            {
                var sequence = (Sequence)node;
                var children = new List<InlineLayout>();
                for (int i = 0; i < sequence.Contents.Count; i++)
                {
                    children.Add(CompileInline(sequence.Contents[i]));
                }
                return new InlineConcat(children);
            }
            if (node is ColorSpan)
            {
                var color = (ColorSpan)node;
                return new StyledInline(
                    CompileInline(color.Contents),
                    color.ForegroundColor,
                    color.BackgroundColor,
                    TextDecoration.None);
            }
            if (node is DecorationSpan)
            {
                var decoration = (DecorationSpan)node;
                return new StyledInline(
                    CompileInline(decoration.Contents),
                    Colors.Transparent,
                    Colors.Transparent,
                    decoration.Decoration);
            }
            if (node is DegradableText)
            {
                var degradable = (DegradableText)node;
                return new AlternativeInline(
                    degradable.Contents,
                    CompileInline(degradable.Fallback));
            }
            if (node is NewLine)
            {
                return HardLine.Instance;
            }

            var lowered = node.Lower();
            if (lowered != null)
            {
                return CompileInline(lowered);
            }

            throw new UnsupportedNodeException(node);
        }

        public BlockLayout CompileBlock(Block node)
        {
            if (node == null)
            {
                return new BlockStack(new BlockLayout[0]);
            }

            if (node is Paragraph)
            {
                return new FlowBlock(
                    CompileInline(((Paragraph)node).Contents),
                    Margins.Paragraph);
            }
            if (node is Stack)
            {
                var stack = (Stack)node;
                var children = new List<BlockLayout>();
                for (int i = 0; i < stack.Contents.Count; i++)
                {
                    children.Add(CompileBlock(stack.Contents[i]));
                }
                return new BlockStack(children);
            }
            if (node is WrapBox)
            {
                var box = (WrapBox)node;
                return new LayoutBox(
                    CompileBlock(box.Contents),
                    box.Wrapping,
                    Alignment.Left,
                    box.LeftMargin,
                    box.RightMargin);
            }
            if (node is IndentBox)
            {
                return new LayoutBox(
                    CompileBlock(((IndentBox)node).Contents),
                    WrappingStrategy.Word,
                    Alignment.Left,
                    4,
                    0);
            }
            if (node is AlignBox)
            {
                var align = (AlignBox)node;
                return new LayoutBox(
                    CompileBlock(align.Contents),
                    WrappingStrategy.Character,
                    align.Alignment,
                    0,
                    0);
            }
            if (node is PrefixBox)
            {
                var prefix = (PrefixBox)node;
                return new PrefixLayout(
                    CompileInline(prefix.Prefix),
                    CompileBlock(prefix.Contents));
            }
            if (node is Diagnostic)
            {
                return CompileDiagnostic((Diagnostic)node);
            }
            if (node is HighlightedSource)
            {
                return new FixedBlock(
                    terminal => HighlightedSourceFormatter.Render(
                        (HighlightedSource)node,
                        terminal,
                        5,
                        highlightColor),
                    Margins.Paragraph);
            }

            var lowered = node.Lower();
            if (lowered != null)
            {
                return CompileBlock(lowered);
            }

            throw new UnsupportedNodeException(node);
        }

        private BlockLayout CompileDiagnostic(Diagnostic diagnostic)
        {
            var titlePart = Text.IsEmpty(diagnostic.Title)
                ? (Inline)""
                : new Sequence(diagnostic.Title, ": ");

            Inline header =
                new Sequence(
                    diagnostic.Origin,
                    ": ",
                    new ColorSpan(diagnostic.Kind + ": ", diagnostic.ThemeColor),
                    titlePart,
                    diagnostic.Message ?? "");

            var blocks = new List<BlockLayout>();
            blocks.Add(
                new FlowBlock(
                    new StyledInline(
                        CompileInline(header),
                        Colors.Transparent,
                        Colors.Transparent,
                        TextDecoration.Bold),
                    diagnostic.Details == null
                        ? Margins.None
                        : new Margins(0, 1)));

            if (diagnostic.Details != null)
            {
                blocks.Add(new LayoutCompiler(diagnostic.ThemeColor).CompileBlock(diagnostic.Details));
            }

            return new BlockStack(blocks);
        }
    }
}
