using System;
using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// Summarizes the forms of a list of options.
    /// </summary>
    public sealed class OptionSetSummary : MarkupNode
    {
        /// <summary>
        /// Creates an option set summary node.
        /// </summary>
        /// <param name="options">The options to summarize.</param>
        /// <param name="printer">The option printer to use.</param>
        public OptionSetSummary(IReadOnlyList<Option> options, OptionPrinter printer)
        {
            this.Options = options;
            this.Printer = printer;
        }

        /// <summary>
        /// Gets the list of options to summarize.
        /// </summary>
        /// <returns>A list of options to summarize.</returns>
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
                var grouped = SortAndGroupByCategory(Options);

                var groupNodes = new List<MarkupNode>();
                foreach (var kvPair in grouped)
                {
                    int optCount = kvPair.Value.Count;

                    var optionFormNodes = new List<MarkupNode>();
                    for (int i = 0; i < optCount; i++)
                    {
                        var docs = kvPair.Value[i].Documentation;
                        var forms = kvPair.Value[i].Forms;
                        int formCount = forms.Count;

                        for (int j = 0; j < formCount; j++)
                        {
                            if (optionFormNodes.Count > 0)
                            {
                                optionFormNodes.Add(" ");
                            }
                            optionFormNodes.Add(
                                DecorationSpan.MakeBold(
                                    Printer.Print(
                                        forms[j],
                                        docs.GetParameters(forms[j]))));
                        }
                    }

                    groupNodes.Add(
                        new Paragraph(
                            new Sequence(
                                new MarkupNode[]
                                {
                                    DecorationSpan.MakeBold(kvPair.Key),
                                    new IndentBox(optionFormNodes)
                                })));
                }
                return new Sequence(groupNodes);
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }

        /// <summary>
        /// Groups a list of options by their category and then sorts
        /// the options in each category by their first forms.
        /// </summary>
        /// <param name="options">The options to sort and group.</param>
        /// <returns>
        /// A sorted dictionary that maps categories to sorted lists of options.
        /// </returns>
        public static SortedDictionary<string, IReadOnlyList<Option>> SortAndGroupByCategory(
            IReadOnlyList<Option> options)
        {
            var results = new SortedDictionary<string, IReadOnlyList<Option>>();
            int optionCount = options.Count;

            // Add all the options to the dictionary.
            for (int i = 0; i < optionCount; i++)
            {
                string category = options[i].Documentation.Category;
                IReadOnlyList<Option> list;
                if (!results.TryGetValue(category, out list))
                {
                    list = new List<Option>();
                    results[category] = list;
                }
                ((List<Option>)list).Add(options[i]);
            }

            // Sort all the sub-lists.
            foreach (var kvPair in results)
            {
                ((List<Option>)kvPair.Value).Sort(CompareByFirstForm);
            }

            return results;
        }

        private static int CompareByFirstForm(Option first, Option second)
        {
            int firstFormCount = first.Forms.Count;
            int secondFormCount = second.Forms.Count;

            if (firstFormCount == 0)
            {
                if (secondFormCount == 0)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (secondFormCount == 0)
            {
                return 1;
            }

            return first.Forms[0].Name.CompareTo(second.Forms[0].Name);
        }
    }
}