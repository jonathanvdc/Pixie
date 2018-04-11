using System;
using System.Collections.Generic;
using System.Linq;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that is rendered as a short manual
    /// on how to use a set of options. Options are classified
    /// by category.
    /// </summary>
    public sealed class OptionSetHelp : MarkupNode
    {
        /// <summary>
        /// Creates an option set help node.
        /// </summary>
        /// <param name="options">The options to create a short manual for.</param>
        /// <param name="printer">The option printer to use.</param>
        public OptionSetHelp(IReadOnlyList<Option> options, OptionPrinter printer)
        {
            this.Options = options;
            this.Printer = printer;
        }

        /// <summary>
        /// Gets the list of options to create a short manual for.
        /// </summary>
        /// <returns>A list of options to create a short manual for.</returns>
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
                var grouped = OptionSetSummary.SortAndGroupByCategory(Options);

                var nodes = new List<MarkupNode>();
                foreach (var kvPair in grouped)
                {
                    // Put the category title in bold.
                    nodes.Add(DecorationSpan.MakeBold(kvPair.Key));

                    // Suffix category title with help for
                    // each option.

                    var optNodes = new List<MarkupNode>();
                    int optCount = kvPair.Value.Count;
                    for (int i = 0; i < optCount; i++)
                    {
                        optNodes.Add(new Paragraph(new OptionHelp(kvPair.Value[i], Printer)));
                    }
                    nodes.Add(new IndentBox(optNodes));
                }
                return new Sequence(nodes);
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }
    }
}
