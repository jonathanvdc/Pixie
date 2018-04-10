using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Represents documentation attached to an option.
    /// </summary>
    public sealed class OptionDocs
    {
        /// <summary>
        /// Creates documentation for an option.
        /// </summary>
        /// <param name="description">
        /// A description of what the option does.
        /// </param>
        /// <param name="parameters">
        /// A list of parameters for each option form.
        /// </param>
        public OptionDocs(
            MarkupNode description,
            IReadOnlyDictionary<OptionForm, IReadOnlyList<OptionParameter>> parameters)
        {
            this.Description = description;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Creates documentation for an option.
        /// </summary>
        /// <param name="description">
        /// A description of what the option does.
        /// </param>
        /// <param name="forms">
        /// A list of forms for the option.
        /// </param>
        /// <param name="parameters">
        /// A common list of parameters for all option forms.
        /// </param>
        public OptionDocs(
            MarkupNode description,
            IReadOnlyList<OptionForm> forms,
            IReadOnlyList<OptionParameter> parameters)
        {
            this.Description = description;
            var paramDocs = new Dictionary<OptionForm, IReadOnlyList<OptionParameter>>();
            for (int i = 0; i < forms.Count; i++)
            {
                paramDocs[forms[i]] = parameters;
            }
            this.Parameters = paramDocs;
        }

        /// <summary>
        /// Gets a description of what the option does.
        /// </summary>
        /// <returns>A description.</returns>
        public MarkupNode Description { get; private set; }

        /// <summary>
        /// Gets a dictionary that maps each option form to
        /// a list of parameter docs for that form.
        /// </summary>
        /// <returns>A list of parameters.</returns>
        public IReadOnlyDictionary<OptionForm, IReadOnlyList<OptionParameter>> Parameters { get; private set; }

        /// <summary>
        /// Gets the parameter documentation for a particular
        /// option form.
        /// </summary>
        /// <param name="form">
        /// The form to get documentation for.
        /// </param>
        /// <returns>
        /// A list of parameter docs, one for each parameter.
        /// </returns>
        public IReadOnlyList<OptionParameter> GetParameters(OptionForm form)
        {
            IReadOnlyList<OptionParameter> parameterDocs;
            if (Parameters.TryGetValue(form, out parameterDocs))
            {
                return parameterDocs;
            }
            else
            {
                return new OptionalOptionParameter[0];
            }
        }
    }
}
