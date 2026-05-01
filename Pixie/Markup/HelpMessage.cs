using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that prints a short manual for a program.
    /// </summary>
    public sealed class HelpMessage : Block
    {
        /// <summary>
        /// Creates a help message that uses the default GNU-style option printer.
        /// </summary>
        /// <param name="summary">The program summary.</param>
        /// <param name="usage">The program usage syntax.</param>
        /// <param name="options">The supported command-line options.</param>
        public HelpMessage(Block summary, Inline usage, IReadOnlyList<Option> options)
            : this(summary, usage, options, GnuOptionPrinter.Instance)
        { }

        /// <summary>
        /// Creates a help message.
        /// </summary>
        /// <param name="summary">The program summary.</param>
        /// <param name="usage">The program usage syntax.</param>
        /// <param name="options">The supported command-line options.</param>
        /// <param name="printer">The option printer to use.</param>
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

        /// <summary>
        /// Gets the program summary.
        /// </summary>
        public Block Summary { get; }

        /// <summary>
        /// Gets the program usage syntax.
        /// </summary>
        public Inline Usage { get; }

        /// <summary>
        /// Gets the supported command-line options.
        /// </summary>
        public IReadOnlyList<Option> Options { get; }

        /// <summary>
        /// Gets the option printer used to render option names and arguments.
        /// </summary>
        public OptionPrinter Printer { get; }

        /// <summary>
        /// Lowers this help message to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            return WrapBox.WordWrap(
                new Stack(
                    new Div(DecorationSpan.MakeBold("DESCRIPTION")),
                    new IndentBox(Summary),
                    new Div(DecorationSpan.MakeBold("USAGE")),
                    new IndentBox(new Paragraph(Usage)),
                    new Div(DecorationSpan.MakeBold("OPTION SUMMARY")),
                    new IndentBox(
                        new Paragraph(
                            "Here is a summary of all the options, grouped by type. "
                            + "Explanations are in the following sections."),
                        new OptionSetSummary(Options, Printer)),
                    new OptionSetHelp(Options, Printer)));
        }
    }
}
