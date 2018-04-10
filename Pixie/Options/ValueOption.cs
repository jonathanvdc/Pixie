using System;
using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Describes a value option: an option takes exactly one argument.
    /// </summary>
    public sealed class ValueOption<T> : Option
    {
        /// <summary>
        /// Creates a value option from an option form,
        /// a function that parses an argument and a
        /// default value.
        /// </summary>
        /// <param name="form">
        /// The option's form.
        /// </param>
        /// <param name="parseArgument">
        /// A function that parses a string argument.
        /// </param>
        /// <param name="defaultValue">
        /// A default value for the value option.
        /// </param>
        public ValueOption(
            OptionForm form,
            Func<OptionForm, string, ILog, T> parseArgument,
            T defaultValue)
            : this(
                new OptionForm[] { form },
                parseArgument,
                defaultValue)
        { }

        /// <summary>
        /// Creates a value option from a list of forms,
        /// a function that parses an argument and a
        /// default value.
        /// </summary>
        /// <param name="forms">
        /// The list of forms that the value option accepts.
        /// </param>
        /// <param name="parseArgument">
        /// A function that parses a string argument.
        /// </param>
        /// <param name="defaultValue">
        /// A default value for the value option.
        /// </param>
        public ValueOption(
            IReadOnlyList<OptionForm> forms,
            Func<OptionForm, string, ILog, T> parseArgument,
            T defaultValue)
            : this(
                forms,
                parseArgument,
                defaultValue,
                OptionDocs.DefaultCategory,
                "",
                new SymbolicOptionParameter("arg", false))
        { }

        private ValueOption(
            IReadOnlyList<OptionForm> forms,
            Func<OptionForm, string, ILog, T> parseArgument,
            T defaultValue,
            string category,
            MarkupNode description,
            OptionParameter parameter)
        {
            this.forms = forms;
            this.parseArgument = parseArgument;
            this.defaultValue = defaultValue;
            this.category = category;
            this.description = description;
            this.parameter = parameter;
        }

        private ValueOption(ValueOption<T> other)
            : this(
                other.forms,
                other.parseArgument,
                other.defaultValue,
                other.category,
                other.description,
                other.parameter)
        { }

        private IReadOnlyList<OptionForm> forms;
        private Func<OptionForm, string, ILog, T> parseArgument;
        private T defaultValue;

        private string category;
        private MarkupNode description;
        private OptionParameter parameter;

        /// <summary>
        /// Creates a copy of this option that is classified under a
        /// particular category.
        /// </summary>
        /// <param name="category">The new option's category.</param>
        /// <returns>An option.</returns>
        public ValueOption<T> WithCategory(string category)
        {
            var result = new ValueOption<T>(this);
            result.category = category;
            return result;
        }

        /// <summary>
        /// Creates a copy of this option that has a particular description.
        /// </summary>
        /// <param name="description">The new option's description.</param>
        /// <returns>An option.</returns>
        public ValueOption<T> WithDescription(MarkupNode description)
        {
            var result = new ValueOption<T>(this);
            result.description = description;
            return result;
        }

        /// <summary>
        /// Creates a copy of this option that has a particular parameter.
        /// </summary>
        /// <param name="parameter">The new option's sole parameter.</param>
        /// <returns>An option.</returns>
        public ValueOption<T> WithParameter(OptionParameter parameter)
        {
            var result = new ValueOption<T>(this);
            result.parameter = parameter;
            return result;
        }

        /// <inheritdoc/>
        public override IReadOnlyList<OptionForm> Forms => forms;

        /// <inheritdoc/>
        public override object DefaultValue => defaultValue;

        /// <inheritdoc/>
        public override OptionDocs Documentation =>
            new OptionDocs(
                category,
                description,
                forms,
                new OptionParameter[] { parameter });

        /// <inheritdoc/>
        public override OptionParser CreateParser(OptionForm form)
        {
            return new ValueOptionParser<T>(form, parseArgument);
        }

        /// <inheritdoc/>
        public override ParsedOption MergeValues(ParsedOption first, ParsedOption second)
        {
            return second;
        }
    }

    /// <summary>
    /// Helps build value options.
    /// </summary>
    public static class ValueOption
    {
        /// <summary>
        /// Creates a string option that takes a single form.
        /// </summary>
        /// <param name="form">The string option's form.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <returns>A string option.</returns>
        public static ValueOption<string> CreateStringOption(
            OptionForm form,
            string defaultValue)
        {
            return new ValueOption<string>(
                form,
                SequenceOption.parseStringArgument,
                defaultValue);
        }

        /// <summary>
        /// Creates a string option that takes a list of forms.
        /// </summary>
        /// <param name="forms">The string option's forms.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <returns>A string option.</returns>
        public static ValueOption<string> CreateStringOption(
            IReadOnlyList<OptionForm> forms,
            string defaultValue)
        {
            return new ValueOption<string>(
                forms,
                SequenceOption.parseStringArgument,
                defaultValue);
        }

        /// <summary>
        /// Creates a 32-bit signed integer option that takes a single form.
        /// </summary>
        /// <param name="form">The 32-bit signed integer option's form.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <returns>A 32-bit signed integer option.</returns>
        public static ValueOption<int> CreateInt32Option(
            OptionForm form,
            int defaultValue)
        {
            return new ValueOption<int>(
                form,
                SequenceOption.parseInt32Argument,
                defaultValue);
        }

        /// <summary>
        /// Creates a 32-bit signed integer option that takes a list of forms.
        /// </summary>
        /// <param name="forms">The 32-bit signed integer option's forms.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <returns>A 32-bit signed integer option.</returns>
        public static ValueOption<int> CreateInt32Option(
            IReadOnlyList<OptionForm> forms,
            int defaultValue)
        {
            return new ValueOption<int>(
                forms,
                SequenceOption.parseInt32Argument,
                defaultValue);
        }
    }

    internal sealed class ValueOptionParser<T> : OptionParser
    {
        public ValueOptionParser(
            OptionForm form,
            Func<OptionForm, string, ILog, T> parseArgument)
        {
            this.form = form;
            this.parseArgument = parseArgument;
            this.argument = null;
        }

        private OptionForm form;
        private Func<OptionForm, string, ILog, T> parseArgument;
        private string argument;

        public override object GetValue(ILog log)
        {
            return parseArgument(form, argument, log);
        }

        public override bool Parse(string argument)
        {
            // Accept exactly one argument.
            if (this.argument == null)
            {
                this.argument = argument;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
