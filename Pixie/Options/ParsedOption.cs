namespace Pixie.Options
{
    /// <summary>
    /// Represents a parsed option.
    /// </summary>
    public struct ParsedOption
    {
        /// <summary>
        /// Creates a parsed option from a form and a value.
        /// </summary>
        /// <param name="form">
        /// The form of the option that was parsed.
        /// </param>
        /// <param name="value">
        /// The parsed arguments to the option.
        /// </param>
        public ParsedOption(OptionForm form, object value)
        {
            this = default(ParsedOption);
            this.Form = form;
            this.Value = value;
        }

        /// <summary>
        /// Gets the form of the option that was parsed.
        /// </summary>
        /// <returns>The parsed form.</returns>
        public OptionForm Form { get; private set; }

        /// <summary>
        /// Gets the value of the option, parsed from the
        /// option's arguments.
        /// </summary>
        /// <returns>The parsed value.</returns>
        public object Value { get; private set; }
    }
}