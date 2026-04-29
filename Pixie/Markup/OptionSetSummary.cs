using System.Collections.Generic;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// Summarizes the forms of a list of options.
    /// </summary>
    public sealed class OptionSetSummary : Block
    {
        public OptionSetSummary(IReadOnlyList<Option> options, OptionPrinter printer)
        {
            this.Options = options;
            this.Printer = printer;
        }

        public IReadOnlyList<Option> Options { get; private set; }

        public OptionPrinter Printer { get; private set; }

        public override Block Lower()
        {
            var grouped = SortAndGroupByCategory(Options);
            var groupNodes = new List<Block>();
            foreach (var kvPair in grouped)
            {
                var optionFormNodes = new List<Inline>();
                for (int i = 0; i < kvPair.Value.Count; i++)
                {
                    var docs = kvPair.Value[i].Documentation;
                    var forms = kvPair.Value[i].Forms;
                    for (int j = 0; j < forms.Count; j++)
                    {
                        if (optionFormNodes.Count > 0)
                        {
                            optionFormNodes.Add(" ");
                        }
                        optionFormNodes.Add(
                            DecorationSpan.MakeBold(
                                Printer.Print(forms[j], docs.GetParameters(forms[j]))));
                    }
                }

                groupNodes.Add(
                    new Stack(
                        new Paragraph(DecorationSpan.MakeBold(kvPair.Key)),
                        new IndentBox(new Paragraph(new Sequence(optionFormNodes)))));
            }
            return new Stack(groupNodes);
        }

        public static SortedDictionary<string, IReadOnlyList<Option>> SortAndGroupByCategory(
            IReadOnlyList<Option> options)
        {
            var results = new SortedDictionary<string, IReadOnlyList<Option>>();
            for (int i = 0; i < options.Count; i++)
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

            foreach (var kvPair in results)
            {
                ((List<Option>)kvPair.Value).Sort(CompareByFirstForm);
            }

            return results;
        }

        private static int CompareByFirstForm(Option first, Option second)
        {
            if (first.Forms.Count == 0)
            {
                return second.Forms.Count == 0 ? 0 : -1;
            }
            if (second.Forms.Count == 0)
            {
                return 1;
            }

            return first.Forms[0].Name.CompareTo(second.Forms[0].Name);
        }
    }
}
