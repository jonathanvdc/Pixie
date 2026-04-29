using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that renders help for a set of options grouped by category.
    /// </summary>
    public sealed class OptionSetHelp : Block
    {
        /// <summary>
        /// Creates help markup for a set of command-line options grouped by category.
        /// </summary>
        /// <param name="options">The options to document.</param>
        /// <param name="printer">The option printer to use.</param>
        public OptionSetHelp(IReadOnlyList<Option> options, OptionPrinter printer)
        {
            this.Options = options;
            this.Printer = printer;
        }

        /// <summary>
        /// Gets the options being documented.
        /// </summary>
        public IReadOnlyList<Option> Options { get; private set; }

        /// <summary>
        /// Gets the option printer used to render option names and arguments.
        /// </summary>
        public OptionPrinter Printer { get; private set; }

        /// <summary>
        /// Lowers this option set help block to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            var grouped = OptionSetSummary.SortAndGroupByCategory(Options);
            var nodes = new List<Block>();
            foreach (var kvPair in grouped)
            {
                nodes.Add(new Paragraph(DecorationSpan.MakeBold(kvPair.Key)));

                var optNodes = new List<Block>();
                for (int i = 0; i < kvPair.Value.Count; i++)
                {
                    optNodes.Add(new OptionHelp(kvPair.Value[i], Printer));
                }
                nodes.Add(new IndentBox(new Stack(optNodes)));
            }
            return new Stack(nodes);
        }
    }
}
