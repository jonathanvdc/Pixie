using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that prints a short manual for a program.
    /// </summary>
    public sealed class HelpMessage : Block
    {
        public HelpMessage(Block summary, Inline usage, IReadOnlyList<Option> options)
            : this(summary, usage, options, GnuOptionPrinter.Instance)
        { }

        public HelpMessage(
            Block summary,
            Inline usage,
            IReadOnlyList<Option> options,
            OptionPrinter printer)
        {
            this.Summary = summary;
            this.Usage = usage;
            this.Options = options;
            this.Printer = printer;
        }

        public Block Summary { get; private set; }

        public Inline Usage { get; private set; }

        public IReadOnlyList<Option> Options { get; private set; }

        public OptionPrinter Printer { get; private set; }

        public override Block Lower()
        {
            return WrapBox.WordWrap(
                new Stack(
                    new Paragraph(DecorationSpan.MakeBold("Description")),
                    new IndentBox(Summary),
                    new Paragraph(DecorationSpan.MakeBold("Usage")),
                    new IndentBox(new Paragraph(Usage)),
                    new Paragraph(DecorationSpan.MakeBold("Option summary")),
                    new IndentBox(
                        new Paragraph(
                            "Here is a summary of all the options, grouped by type. "
                            + "Explanations are in the following sections."),
                        new OptionSetSummary(Options, Printer)),
                    new OptionSetHelp(Options, Printer)));
        }
    }
}
