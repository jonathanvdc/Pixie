using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Defines a common interface for option printers:
    /// classes that print option forms and their parameters
    /// according to a particular scheme, for documentation
    /// purposes.
    /// </summary>
    public abstract class OptionPrinter
    {
        /// <summary>
        /// Prints an option form that takes a list of parameters.
        /// </summary>
        /// <param name="form">The option form to print.</param>
        /// <param name="parameters">
        /// The list of parameters for the option form.
        /// </param>
        /// <returns>
        /// A markup node that represents the option.
        /// </returns>
        public abstract MarkupNode Print(
            OptionForm form,
            IReadOnlyList<OptionParameter> parameters);
    }
}
