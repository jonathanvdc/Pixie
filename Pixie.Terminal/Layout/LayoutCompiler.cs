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
            switch (node)
            {
                case null:
                    return EmptyInline.Instance;
                case Text text when Text.IsEmpty(text):
                    return EmptyInline.Instance;
                case Sequence sequence when Text.IsEmpty(sequence):
                    return EmptyInline.Instance;
                case Text text:
                    return new TextRun(text.Contents);
                case Sequence sequence:
                    return CompileSequence(sequence);
                case ColorSpan color:
                    return new StyledInline(
                        CompileInline(color.Contents),
                        color.ForegroundColor,
                        color.BackgroundColor,
                        TextDecoration.None);
                case DecorationSpan decoration:
                    return new StyledInline(
                        CompileInline(decoration.Contents),
                        Colors.Transparent,
                        Colors.Transparent,
                        decoration.Decoration);
                case DegradableText degradable:
                    return new AlternativeInline(
                        degradable.Contents,
                        CompileInline(degradable.Fallback));
                case NewLine _:
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
            switch (node)
            {
                case null:
                    return new BlockStack(new BlockLayout[0]);
                case Paragraph paragraph:
                    return new FlowBlock(
                        CompileInline(paragraph.Contents),
                        Margins.Paragraph);
                case Stack stack:
                    return CompileStack(stack);
                case WrapBox box:
                    return new LayoutBox(
                        CompileBlock(box.Contents),
                        box.Wrapping,
                        Alignment.Left,
                        box.LeftMargin,
                        box.RightMargin);
                case IndentBox indent:
                    return new LayoutBox(
                        CompileBlock(indent.Contents),
                        WrappingStrategy.Word,
                        Alignment.Left,
                        4,
                        0);
                case AlignBox align:
                    return new LayoutBox(
                        CompileBlock(align.Contents),
                        WrappingStrategy.Character,
                        align.Alignment,
                        0,
                        0);
                case PrefixBox prefix:
                    return new PrefixLayout(
                        CompileInline(prefix.Prefix),
                        CompileBlock(prefix.Contents));
                case Diagnostic diagnostic:
                    return CompileDiagnostic(diagnostic);
                case HighlightedSource source:
                    return new FixedBlock(
                        terminal => HighlightedSourceFormatter.Render(
                            source,
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

        private InlineLayout CompileSequence(Sequence sequence)
        {
            var children = new List<InlineLayout>();
            for (int i = 0; i < sequence.Contents.Count; i++)
            {
                children.Add(CompileInline(sequence.Contents[i]));
            }
            return new InlineConcat(children);
        }

        private BlockLayout CompileStack(Stack stack)
        {
            var children = new List<BlockLayout>();
            for (int i = 0; i < stack.Contents.Count; i++)
            {
                children.Add(CompileBlock(stack.Contents[i]));
            }
            return new BlockStack(children);
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
