using System;
using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Describes a flag option: a Boolean option whose value is set
    /// by mentioning one of the option's forms. Flag options take no
    /// arguments.
    /// </summary>
    public sealed class FlagOption : Option
    {
        /// <summary>
        /// Creates a flag option from a positive form.
        /// The flag's default value is <c>false</c>.
        /// </summary>
        /// <param name="positiveForm">
        /// A positive form for the flag option.
        /// </param>
        public FlagOption(
            OptionForm positiveForm)
            : this(
                new OptionForm[] { positiveForm },
                new OptionForm[] { },
                false)
        { }

        /// <summary>
        /// Creates a flag option from a positive form,
        /// a negative form and a default value.
        /// </summary>
        /// <param name="positiveForm">
        /// A positive form for the flag option.
        /// </param>
        /// <param name="negativeForm">
        /// A negative form for the flag option.
        /// </param>
        /// <param name="defaultValue">
        /// A default value for the flag option.
        /// </param>
        public FlagOption(
            OptionForm positiveForm,
            OptionForm negativeForm,
            bool defaultValue)
            : this(
                new OptionForm[] { positiveForm },
                new OptionForm[] { negativeForm },
                defaultValue)
        { }

        /// <summary>
        /// Creates a flag option from a list of positive forms,
        /// a list of negative forms and a default value.
        /// </summary>
        /// <param name="positiveForms">
        /// A list of positive forms for the flag option.
        /// </param>
        /// <param name="negativeForms">
        /// A list of negative forms for the flag option.
        /// </param>
        /// <param name="defaultValue">
        /// A default value for the flag option.
        /// </param>
        public FlagOption(
            IReadOnlyList<OptionForm> positiveForms,
            IReadOnlyList<OptionForm> negativeForms,
            bool defaultValue)
            : this(
                positiveForms,
                negativeForms,
                defaultValue,
                "")
        { }

        private FlagOption(
            IReadOnlyList<OptionForm> positiveForms,
            IReadOnlyList<OptionForm> negativeForms,
            bool defaultValue,
            MarkupNode description)
        {
            this.PositiveForms = positiveForms;
            this.NegativeForms = negativeForms;
            this.positiveFormSet = new HashSet<OptionForm>(positiveForms);
            this.defaultVal = defaultValue;
            this.allForms = new List<OptionForm>(positiveForms);
            this.allForms.AddRange(negativeForms);
            this.category = OptionDocs.DefaultCategory;
            this.description = description;
        }

        private FlagOption(FlagOption other)
        {
            this.PositiveForms = other.PositiveForms;
            this.NegativeForms = other.NegativeForms;
            this.positiveFormSet = other.positiveFormSet;
            this.defaultVal = other.defaultVal;
            this.allForms = other.allForms;
            this.description = other.description;
        }

        /// <summary>
        /// Gets a list of positive forms for this flag option.
        /// </summary>
        /// <returns>A list of positive forms for this flag option.</returns>
        public IReadOnlyList<OptionForm> PositiveForms { get; private set; }

        /// <summary>
        /// Gets a list of negative forms for this flag option.
        /// </summary>
        /// <returns>A list of negative forms for this flag option.</returns>
        public IReadOnlyList<OptionForm> NegativeForms { get; private set; }

        private HashSet<OptionForm> positiveFormSet;
        private List<OptionForm> allForms;
        private bool defaultVal;

        private string category;
        private MarkupNode description;

        /// <inheritdoc/>
        public override IReadOnlyList<OptionForm> Forms => allForms;

        /// <inheritdoc/>
        public override object DefaultValue => defaultVal;

        /// <inheritdoc/>
        public override OptionDocs Documentation =>
            new OptionDocs(
                category,
                description,
                new Dictionary<OptionForm, IReadOnlyList<OptionParameter>>());

        /// <summary>
        /// Creates a copy of this option that is classified under a
        /// particular category.
        /// </summary>
        /// <param name="category">The new option's category.</param>
        /// <returns>An option.</returns>
        public FlagOption WithCategory(string category)
        {
            var result = new FlagOption(this);
            result.category = category;
            return result;
        }

        /// <summary>
        /// Creates a copy of this option that has a particular description.
        /// </summary>
        /// <param name="description">The new option's description.</param>
        /// <returns>An option.</returns>
        public FlagOption WithDescription(MarkupNode description)
        {
            var result = new FlagOption(this);
            result.description = description;
            return result;
        }

        /// <inheritdoc/>
        public override OptionParser CreateParser(OptionForm form)
        {
            if (positiveFormSet.Contains(form))
            {
                return new FlagOptionParser(true);
            }
            else
            {
                return new FlagOptionParser(false);
            }
        }

        /// <inheritdoc/>
        public override ParsedOption MergeValues(ParsedOption first, ParsedOption second)
        {
            // Listen to the last flag provided by the user.
            return second;
        }

        /// <summary>
        /// Creates a simple flag option that is turned off
        /// by default and is turned on when one or more of
        /// its forms is encountered.
        /// </summary>
        /// <param name="forms">The option forms to accept.</param>
        /// <returns>A flag option.</returns>
        public static FlagOption CreateFlagOption(
            IReadOnlyList<OptionForm> forms)
        {
            return new FlagOption(forms, new OptionForm[] { }, false);
        }

        /// <summary>
        /// Creates a simple flag option that is turned off
        /// by default and is turned on when one or more of
        /// its forms is encountered.
        /// </summary>
        /// <param name="forms">The option forms to accept.</param>
        /// <returns>A flag option.</returns>
        public static FlagOption CreateFlagOption(
            params OptionForm[] forms)
        {
            return CreateFlagOption((IReadOnlyList<OptionForm>)forms);
        }
    }

    internal sealed class FlagOptionParser : OptionParser
    {
        public FlagOptionParser(bool value)
        {
            this.value = value;
        }

        private bool value;

        public override object GetValue(ILog log)
        {
            return value;
        }

        public override bool Parse(string argument)
        {
            // Flags don't take any arguments.
            return false;
        }
    }
}
