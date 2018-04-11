using System;
using System.Collections.Generic;
using System.Text;
using Pixie.Options;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that is rendered as a short manual
    /// on how to use an option.
    /// </summary>
    public sealed class OptionHelp : MarkupNode
    {
        /// <summary>
        /// Creates an option help node.
        /// </summary>
        /// <param name="option">
        /// The option to create a short manual for.
        /// </param>
        /// <param name="printer">
        /// The option printer to use for rendering
        /// option forms.
        /// </param>
        public OptionHelp(Option option, OptionPrinter printer)
        {
            this.Option = option;
            this.Printer = printer;
        }

        /// <summary>
        /// Gets the option to document.
        /// </summary>
        /// <returns>The option to document.</returns>
        public Option Option { get; private set; }

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
                // The output of this markup node is inspired by
                // man pages, chiefly GCC's and GHC's.
                //
                // For most options, both the GCC and GHC man pages
                // agree on the following format:
                //
                // --help,-?
                //     Display help
                //
                // The GCC man page seems to print single-character
                // and two-character short-form options like so:
                //
                // -c  Compile or assemble the source files, but do not link. The linking
                //     stage simply is not done. The ultimate output is in the form of an
                //     object file for each source file.
                //
                // man GHC also does that for larger options. man GCC
                // can't because it uses four speces of identation
                // whereas the GHC man page uses eight spaces.
                //
                // The rule for prefixing a paragraph with an option
                // seems to be that prefixing only happens if the
                // printed option plus one space character fit in the
                // spacing before the paragraph.
                //
                // We will use a prefix box if there is only one form,
                // that form is sufficiently short and the form does
                // not take any parameters.

                var forms = Option.Forms;
                var docs = Option.Documentation;
                if (forms.Count == 1
                    && forms[0].ToString().Length <= 3
                    && docs.GetParameters(forms[0]).Count == 0)
                {
                    // Use a prefix box.

                    var form = forms[0];

                    // Create a padding string.
                    var sb = new StringBuilder();
                    sb.Append(' ', 4 - form.ToString().Length);

                    // Wrap the description in a prefix box.
                    return new PrefixBox(
                        new Sequence(
                            Printer.Print(form, new OptionParameter[0]),
                            sb.ToString()),
                        docs.Description);
                }
                else
                {
                    // Use an indent box.

                    // Create a header containing all of the option's forms.
                    var formNodes = new List<MarkupNode>();
                    int formCount = forms.Count;
                    for (int i = 0; i < formCount; i++)
                    {
                        if (i > 0)
                        {
                            formNodes.Add(", ");
                        }

                        // Print option forms in bold.
                        formNodes.Add(
                            DecorationSpan.MakeBold(
                                Printer.Print(
                                    forms[i],
                                    docs.GetParameters(forms[i]))));
                    }

                    // Suffix that with an indent box containing the description.
                    formNodes.Add(new IndentBox(docs.Description));

                    return new Sequence(formNodes);
                }
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }
    }
}