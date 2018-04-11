using System;
using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A node that prints a short manual for a program.
    /// </summary>
    public sealed class HelpMessage : MarkupNode
    {
        /// <summary>
        /// Creates a short manual for a program.
        /// </summary>
        /// <param name="summary">
        /// A summary of the program's functionality.
        /// </param>
        /// <param name="usage">
        /// A description of how the program should be used.
        /// </param>
        /// <param name="options">
        /// A list of options accepted by the program.
        /// </param>
        public HelpMessage(
            MarkupNode summary,
            MarkupNode usage,
            IReadOnlyList<Option> options)
            : this(summary, usage, options, GnuOptionPrinter.Instance)
        { }

        /// <summary>
        /// Creates a short manual for a program.
        /// </summary>
        /// <param name="summary">
        /// A summary of the program's functionality.
        /// </param>
        /// <param name="usage">
        /// A description of how the program should be used.
        /// </param>
        /// <param name="options">
        /// A list of options accepted by the program.
        /// </param>
        /// <param name="printer">
        /// The option printer to use. The option printer
        /// defines the syntax for which options are formatted.
        /// </param>
        public HelpMessage(
            MarkupNode summary,
            MarkupNode usage,
            IReadOnlyList<Option> options,
            OptionPrinter printer)
        {
            this.Summary = summary;
            this.Usage = usage;
            this.Options = options;
            this.Printer = printer;
        }

        /// <summary>
        /// Gets a summary of the program's functionality.
        /// </summary>
        /// <returns>A summary of the program's functionality.</returns>
        public MarkupNode Summary { get; private set; }

        /// <summary>
        /// Gets a description of how the program should be used.
        /// </summary>
        /// <returns>A usage description.</returns>
        public MarkupNode Usage { get; private set; }

        /// <summary>
        /// Gets a list of options accepted by the program.
        /// </summary>
        /// <returns>The options accepted by the program.</returns>
        public IReadOnlyList<Option> Options { get; private set; }

        /// <summary>
        /// Gets the option printer to use.
        /// </summary>
        /// <returns>The option printer.</returns>
        public OptionPrinter Printer { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback
        {
            get
            {
                // We'll compose a message that looks more or
                // less like this:
                //
                // Summary
                //     <summary node>
                //
                // Usage
                //     <usage node>
                //
                // Option summary
                //     <option set summary node>
                //
                // <option set help node>

                return WrapBox.WordWrap(
                    new Sequence(
                        new Paragraph(
                            DecorationSpan.MakeBold("Description"),
                            new IndentBox(Summary)),
                        new Paragraph(
                            DecorationSpan.MakeBold("Usage"),
                            new IndentBox(Usage)),
                        new Paragraph(
                            DecorationSpan.MakeBold("Option summary"),
                            new IndentBox(
                                new Box(
                                    "Here is a summary of all the options, grouped by type. " +
                                    "Explanations are in the following sections."),
                                new OptionSetSummary(Options, Printer))),
                        new OptionSetHelp(Options, Printer)));
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            var newSummary = mapping(Summary);
            var newUsage = mapping(Usage);

            if (newSummary == Summary && newUsage == Usage)
            {
                return this;
            }
            else
            {
                return new HelpMessage(newSummary, newUsage, Options, Printer);
            }
        }
    }
}
