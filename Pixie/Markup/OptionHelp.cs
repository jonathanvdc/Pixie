using System.Collections.Generic;
using System.Text;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A block that renders a short manual for one option.
    /// </summary>
    public sealed class OptionHelp : Block
    {
        /// <summary>
        /// Creates help markup for one command-line option.
        /// </summary>
        /// <param name="option">The option to document.</param>
        /// <param name="printer">The option printer to use.</param>
        public OptionHelp(Option option, OptionPrinter printer)
        {
            this.Option = option;
            this.Printer = printer;
        }

        /// <summary>
        /// Gets the option being documented.
        /// </summary>
        public Option Option { get; private set; }

        /// <summary>
        /// Gets the option printer used to render option names and arguments.
        /// </summary>
        public OptionPrinter Printer { get; private set; }

        /// <summary>
        /// Lowers this option help block to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            var forms = Option.Forms;
            var docs = Option.Documentation;
            if (forms.Count == 1
                && forms[0].ToString().Length <= 3
                && docs.GetParameters(forms[0]).Count == 0)
            {
                var form = forms[0];
                var sb = new StringBuilder();
                sb.Append(' ', 4 - form.ToString().Length);

                return new PrefixBox(
                    new Sequence(
                        Printer.Print(form, new OptionParameter[0]),
                        sb.ToString()),
                    docs.Description);
            }

            var formNodes = new List<Inline>();
            for (int i = 0; i < forms.Count; i++)
            {
                if (i > 0)
                {
                    formNodes.Add(", ");
                }

                formNodes.Add(
                    DecorationSpan.MakeBold(
                        Printer.Print(
                            forms[i],
                            docs.GetParameters(forms[i]))));
            }

            return new Stack(
                new Div(new Sequence(formNodes)),
                new IndentBox(docs.Description));
        }
    }
}
