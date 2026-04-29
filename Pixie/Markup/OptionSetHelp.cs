using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that renders help for a set of options grouped by category.
    /// </summary>
    public sealed class OptionSetHelp : Block
    {
        public OptionSetHelp(IReadOnlyList<Option> options, OptionPrinter printer)
        {
            this.Options = options;
            this.Printer = printer;
        }

        public IReadOnlyList<Option> Options { get; private set; }

        public OptionPrinter Printer { get; private set; }

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
