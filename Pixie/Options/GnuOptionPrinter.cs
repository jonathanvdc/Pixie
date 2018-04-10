using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Options
{
    /// <summary>
    /// An option printer for GNU-style options.
    /// </summary>
    public sealed class GnuOptionPrinter : OptionPrinter
    {
        private GnuOptionPrinter()
        { }

        /// <summary>
        /// Gets an instance of a GNU-style option printer.
        /// </summary>
        /// <returns>An option printer.</returns>
        public static readonly GnuOptionPrinter Instance =
            new GnuOptionPrinter();

        /// <inheritdoc/>
        public override MarkupNode Print(
            OptionForm form,
            IReadOnlyList<OptionParameter> parameters)
        {
            int paramCount = parameters.Count;
            if (paramCount == 1)
            {
                var symbolicParam = parameters[0] as SymbolicOptionParameter;
                if (!symbolicParam.IsVarargs)
                {
                    if (form.IsShort && form.Name.Length == 1)
                    {
                        // Concatenated syntax. Example: -g<n>
                        return new Sequence(
                            form.ToString(),
                            parameters[0].Representation);
                    }
                    else
                    {
                        // Equals-sign syntax. Example: -ferror-limit=<n>
                        return new Sequence(
                            form.ToString() + "=",
                            parameters[0].Representation);
                    }
                }
            }

            // General syntax. Example: -x language files...
            var nodes = new List<MarkupNode>();
            nodes.Add(form.ToString());
            for (int i = 0; i < paramCount; i++)
            {
                nodes.Add(" ");
                nodes.Add(parameters[i].Representation);
            }

            return new Sequence(nodes);
        }
    }
}