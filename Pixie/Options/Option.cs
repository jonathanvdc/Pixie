using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Describes a command-line option.
    /// </summary>
    public abstract class Option
    {
        /// <summary>
        /// Creates an option from a description.
        /// </summary>
        /// <param name="description">
        /// A description of the option's functionality.
        /// </param>
        public Option(MarkupNode description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets a description of what the option does.
        /// </summary>
        /// <returns>A description.</returns>
        public MarkupNode Description { get; private set; }

        /// <summary>
        /// Gets a list of all forms this option accepts.
        /// </summary>
        /// <returns>The option's forms.</returns>
        public abstract IReadOnlyList<OptionForm> Forms { get; }

        /// <summary>
        /// Gets the option's default value.
        /// </summary>
        /// <returns>The default value.</returns>
        public abstract object DefaultValue { get; }

        /// <summary>
        /// Create a parser for one of this option's forms.
        /// </summary>
        /// <param name="form">The form to parse.</param>
        /// <returns>An option parser.</returns>
        public abstract OptionParser CreateParser(OptionForm form);

        /// <summary>
        /// Merges the values of two parsed options. Both have
        /// forms that belong to this option. These forms may
        /// or may not be the same.
        /// </summary>
        /// <param name="first">The first parsed option.</param>
        /// <param name="second">The second parsed option.</param>
        /// <returns>A composite parsed option.</returns>
        public abstract ParsedOption MergeValues(
            ParsedOption first,
            ParsedOption second);
    }

    /// <summary>
    /// Describes a parser for a command-line option.
    /// </summary>
    public abstract class OptionParser
    {
        /// <summary>
        /// Parses an argument to the option.
        /// </summary>
        /// <param name="argument">
        /// An argument to the option, as a string.
        /// </param>
        /// <returns>
        /// <c>true</c> if the argument was accepted as part of the option;
        /// otherwise, <c>false</c> is returned and parsing stops for the option.
        /// </returns>
        public abstract bool Parse(string argument);

        /// <summary>
        /// Gets the value that was parsed by the option parser.
        /// </summary>
        /// <param name="log">A log to which errors can be sent.</param>
        /// <returns>The resulting value.</returns>
        public abstract object GetValue(ILog log);
    }
}