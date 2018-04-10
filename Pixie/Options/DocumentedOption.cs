using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// A data structure that contains an option as well as its
    /// documentation.
    /// </summary>
    public sealed class DocumentedOption
    {
        /// <summary>
        /// Creates a documented option.
        /// </summary>
        /// <param name="option">The option that is documented.</param>
        /// <param name="parameterNames">The names of the option's parameters.</param>
        /// <param name="description">The option's description.</param>
        public DocumentedOption(
            Option option,
            IReadOnlyList<MarkupNode> parameterNames,
            MarkupNode description)
        {
            this.Option = option;
            this.ParameterNames = parameterNames;
            this.Description = description;
        }

        /// <summary>
        /// Creates a documented option that takes no arguments.
        /// </summary>
        /// <param name="option">The option that is documented.</param>
        /// <param name="description">The option's description.</param>
        public DocumentedOption(
            Option option,
            MarkupNode description)
            : this(option, new MarkupNode[0], description)
        { }

        /// <summary>
        /// Gets the option that is documented.
        /// </summary>
        /// <returns>The option.</returns>
        public Option Option { get; private set; }

        /// <summary>
        /// Gets the names of the parameters the option takes, as markup nodes.
        /// </summary>
        /// <returns>The names of the parameters the option takes.</returns>
        public IReadOnlyList<MarkupNode> ParameterNames { get; private set; }

        /// <summary>
        /// Gets a description of what the option does.
        /// </summary>
        /// <returns>A description.</returns>
        public MarkupNode Description { get; private set; }
    }
}