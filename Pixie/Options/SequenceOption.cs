using System;
using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Options
{
    /// <summary>
    /// Describes a sequence option: an option that can take any number
    /// of arguments. Each argument is parsed identically and the result
    /// of the option is formatted as an IReadOnlyList&lt;T&gt;.
    /// </summary>
    public sealed class SequenceOption<T> : Option
    {
        /// <summary>
        /// Creates a sequence option from an option form
        /// and a function that parses a single argument.
        /// </summary>
        /// <param name="form">
        /// The option's form.
        /// </param>
        /// <param name="parseArgument">
        /// A function that parses a single string argument.
        /// </param>
        public SequenceOption(
            OptionForm form,
            Func<OptionForm, string, ILog, T> parseArgument)
            : this(
                new OptionForm[] { form },
                parseArgument)
        { }

        /// <summary>
        /// Creates a sequence option from a list of forms
        /// and a function that parses a single argument.
        /// </summary>
        /// <param name="forms">
        /// The list of forms that the sequence option accepts.
        /// </param>
        /// <param name="parseArgument">
        /// A function that parses a single string argument.
        /// </param>
        public SequenceOption(
            IReadOnlyList<OptionForm> forms,
            Func<OptionForm, string, ILog, T> parseArgument)
            : this(
                forms,
                parseArgument,
                OptionDocs.DefaultCategory,
                "",
                new OptionParameter[]
                {
                    new SymbolicOptionParameter("arg", true)
                })
        { }

        private SequenceOption(
            IReadOnlyList<OptionForm> forms,
            Func<OptionForm, string, ILog, T> parseArgument,
            string category,
            MarkupNode description,
            IReadOnlyList<OptionParameter> parameters)
        {
            this.forms = forms;
            this.parseArgument = parseArgument;
            this.category = category;
            this.description = description;
            this.parameters = parameters;
        }

        private SequenceOption(SequenceOption<T> other)
            : this(
                other.forms,
                other.parseArgument,
                other.category,
                other.description,
                other.parameters)
        { }

        private IReadOnlyList<OptionForm> forms;
        private Func<OptionForm, string, ILog, T> parseArgument;

        private string category;
        private MarkupNode description;
        private IReadOnlyList<OptionParameter> parameters;

        /// <summary>
        /// Creates a copy of this option that is classified under a
        /// particular category.
        /// </summary>
        /// <param name="category">The new option's category.</param>
        /// <returns>An option.</returns>
        public SequenceOption<T> WithCategory(string category)
        {
            var result = new SequenceOption<T>(this);
            result.category = category;
            return result;
        }

        /// <summary>
        /// Creates a copy of this option that has a particular description.
        /// </summary>
        /// <param name="description">The new option's description.</param>
        /// <returns>An option.</returns>
        public SequenceOption<T> WithDescription(MarkupNode description)
        {
            var result = new SequenceOption<T>(this);
            result.description = description;
            return result;
        }

        /// <summary>
        /// Creates a copy of this option that has a particular parameter list.
        /// </summary>
        /// <param name="parameters">The new option's parameter list.</param>
        /// <returns>An option.</returns>
        public SequenceOption<T> WithParameters(IReadOnlyList<OptionParameter> parameters)
        {
            var result = new SequenceOption<T>(this);
            result.parameters = parameters;
            return result;
        }

        /// <summary>
        /// Creates a copy of this option that has a particular parameter list.
        /// </summary>
        /// <param name="parameters">The new option's parameter list.</param>
        /// <returns>An option.</returns>
        public SequenceOption<T> WithParameters(params OptionParameter[] parameters)
        {
            return WithParameters((IReadOnlyList<OptionParameter>)parameters);
        }

        /// <inheritdoc/>
        public override IReadOnlyList<OptionForm> Forms => forms;

        /// <inheritdoc/>
        public override object DefaultValue => new T[] { };

        /// <inheritdoc/>
        public override OptionDocs Documentation =>
            new OptionDocs(
                category,
                description,
                forms,
                parameters);

        /// <inheritdoc/>
        public override OptionParser CreateParser(OptionForm form)
        {
            return new SequenceOptionParser<T>(form, parseArgument);
        }

        /// <inheritdoc/>
        public override ParsedOption MergeValues(
            ParsedOption first, ParsedOption second)
        {
            // Merge the sequences.
            var result = new List<T>((IReadOnlyList<T>)first.Value);
            result.AddRange((IReadOnlyList<T>)second.Value);
            return new ParsedOption(first.Form, result);
        }
    }

    /// <summary>
    /// Helps build sequence options.
    /// </summary>
    public static class SequenceOption
    {
        /// <summary>
        /// Creates a string-sequence option that takes a single form.
        /// </summary>
        /// <param name="form">The string-sequence option's form.</param>
        /// <returns>A string-sequence option.</returns>
        public static SequenceOption<string> CreateStringOption(
            OptionForm form)
        {
            return new SequenceOption<string>(
                form, parseStringArgument);
        }

        /// <summary>
        /// Creates a string-sequence option that takes a list of forms.
        /// </summary>
        /// <param name="forms">The string-sequence option's forms.</param>
        /// <returns>A string-sequence option.</returns>
        public static SequenceOption<string> CreateStringOption(
            IReadOnlyList<OptionForm> forms)
        {
            return new SequenceOption<string>(
                forms, parseStringArgument);
        }

        /// <summary>
        /// Creates a 32-bit integer--sequence option that takes a single form.
        /// </summary>
        /// <param name="form">The 32-bit integer--sequence option's form.</param>
        /// <returns>A 32-bit integer--sequence option.</returns>
        public static SequenceOption<int> CreateInt32Option(
            OptionForm form)
        {
            return new SequenceOption<int>(
                form, parseInt32Argument);
        }

        /// <summary>
        /// Creates a 32-bit integer--sequence option that takes a list of forms.
        /// </summary>
        /// <param name="forms">The 32-bit integer--sequence option's forms.</param>
        /// <returns>A 32-bit integer--sequence option.</returns>
        public static SequenceOption<int> CreateInt32Option(
            IReadOnlyList<OptionForm> forms)
        {
            return new SequenceOption<int>(
                forms, parseInt32Argument);
        }

        internal static string parseStringArgument(
            OptionForm form, string argument, ILog log)
        {
            return argument;
        }

        internal static int parseInt32Argument(
            OptionForm form, string argument, ILog log)
        {
            int result;
            if (!int.TryParse(argument, out result))
            {
                log.Log(
                    new LogEntry(
                        Severity.Error,
                        "option error",
                        "argument to ",
                        Quotation.CreateBoldQuotation(form.ToString()),
                        " should be an integer; got ",
                        argument == null
                            ? (MarkupNode)"nothing."
                            : new Sequence(
                                Quotation.CreateBoldQuotation(argument),
                                ".")));
            }
            return result;
        }
    }

    internal sealed class SequenceOptionParser<T> : OptionParser
    {
        public SequenceOptionParser(
            OptionForm form,
            Func<OptionForm, string, ILog, T> parseArgument)
        {
            this.form = form;
            this.parseArgument = parseArgument;
            this.arguments = new List<string>();
        }

        private OptionForm form;
        private Func<OptionForm, string, ILog, T> parseArgument;
        private List<string> arguments;

        public override object GetValue(ILog log)
        {
            var results = new T[arguments.Count];
            for (int i = 0; i < arguments.Count; i++)
            {
                results[i] = parseArgument(form, arguments[i], log);
            }
            return results;
        }

        public override bool Parse(string argument)
        {
            // Take as many arguments as you can.
            arguments.Add(argument);
            return true;
        }
    }
}
