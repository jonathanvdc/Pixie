using System;
using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Describes a command-line option.
    /// </summary>
    public abstract class Option
    {
        /// <summary>
        /// Creates a simple flag option from command-line spellings such as
        /// <c>-h</c> or <c>--help</c>.
        /// </summary>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A flag option.</returns>
        public static FlagOption Flag(params string[] forms)
        {
            return FlagOption.CreateFlagOption(ParseForms(forms));
        }

        /// <summary>
        /// Creates a flag option with explicit positive and negative forms.
        /// </summary>
        /// <param name="positiveForm">The form that enables the flag.</param>
        /// <param name="negativeForm">The form that disables the flag.</param>
        /// <param name="defaultValue">The default value for the flag.</param>
        /// <returns>A flag option.</returns>
        public static FlagOption Toggle(
            string positiveForm,
            string negativeForm,
            bool defaultValue = false)
        {
            return new FlagOption(
                OptionForm.Parse(positiveForm),
                OptionForm.Parse(negativeForm),
                defaultValue);
        }

        /// <summary>
        /// Creates a string option from command-line spellings such as
        /// <c>--color</c>.
        /// </summary>
        /// <param name="defaultValue">The option's default value.</param>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A string option.</returns>
        public static ValueOption<string> StringWithDefault(
            string defaultValue,
            params string[] forms)
        {
            return ValueOption.CreateStringOption(ParseForms(forms), defaultValue);
        }

        /// <summary>
        /// Creates a string option with an empty-string default value.
        /// </summary>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A string option.</returns>
        public static ValueOption<string> String(params string[] forms)
        {
            return StringWithDefault("", forms);
        }

        /// <summary>
        /// Creates a 32-bit integer option from command-line spellings such as
        /// <c>-O</c>.
        /// </summary>
        /// <param name="defaultValue">The option's default value.</param>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A 32-bit integer option.</returns>
        public static ValueOption<int> Int32WithDefault(
            int defaultValue,
            params string[] forms)
        {
            return ValueOption.CreateInt32Option(ParseForms(forms), defaultValue);
        }

        /// <summary>
        /// Creates a 32-bit integer option with a default value of zero.
        /// </summary>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A 32-bit integer option.</returns>
        public static ValueOption<int> Int32(params string[] forms)
        {
            return Int32WithDefault(0, forms);
        }

        /// <summary>
        /// Creates a typed value option from command-line spellings.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="parseArgument">A function that parses the option's argument.</param>
        /// <param name="defaultValue">The option's default value.</param>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A typed option.</returns>
        public static ValueOption<T> Value<T>(
            Func<OptionForm, string, ILog, T> parseArgument,
            T defaultValue,
            params string[] forms)
        {
            return ValueOption.CreateOption(
                ParseForms(forms),
                parseArgument,
                defaultValue);
        }

        /// <summary>
        /// Creates a string sequence option from command-line spellings such as
        /// <c>--file</c>.
        /// </summary>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A string sequence option.</returns>
        public static SequenceOption<string> StringSequence(params string[] forms)
        {
            return SequenceOption.CreateStringOption(ParseForms(forms));
        }

        /// <summary>
        /// Creates a 32-bit integer sequence option from command-line spellings.
        /// </summary>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A 32-bit integer sequence option.</returns>
        public static SequenceOption<int> Int32Sequence(params string[] forms)
        {
            return SequenceOption.CreateInt32Option(ParseForms(forms));
        }

        /// <summary>
        /// Creates a typed sequence option from command-line spellings.
        /// </summary>
        /// <typeparam name="T">The parsed element type.</typeparam>
        /// <param name="parseArgument">A function that parses each argument.</param>
        /// <param name="forms">The forms accepted by the option.</param>
        /// <returns>A typed sequence option.</returns>
        public static SequenceOption<T> Sequence<T>(
            Func<OptionForm, string, ILog, T> parseArgument,
            params string[] forms)
        {
            return SequenceOption.CreateOption(
                ParseForms(forms),
                parseArgument);
        }

        /// <summary>
        /// Gets the option's documentation.
        /// </summary>
        /// <returns>The documentation for the option.</returns>
        public abstract OptionDocs Documentation { get; }

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

        private static IReadOnlyList<OptionForm> ParseForms(
            IReadOnlyList<string> forms)
        {
            var results = new OptionForm[forms.Count];
            for (int i = 0; i < forms.Count; i++)
            {
                results[i] = OptionForm.Parse(forms[i]);
            }
            return results;
        }
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
