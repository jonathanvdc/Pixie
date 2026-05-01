using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Represents a set of parsed options.
    /// </summary>
    public struct OptionSet
    {
        /// <summary>
        /// Creates a parsed option set from a mapping of
        /// options to parsed options.
        /// </summary>
        /// <param name="contents">A mapping of options to parsed options.</param>
        public OptionSet(IReadOnlyDictionary<Option, ParsedOption> contents)
        {
            this = default(OptionSet);
            this.contents = contents;
        }

        private IReadOnlyDictionary<Option, ParsedOption> contents;

        /// <summary>
        /// Tells if this set of parsed options includes a
        /// particular option.
        /// </summary>
        /// <param name="key">The option to examine.</param>
        /// <returns>
        /// <c>true</c> if a form of the option has been parsed;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsOption(Option key)
        {
            return contents.ContainsKey(key);
        }

        /// <summary>
        /// Looks for the parsed option that corresponds to a particular
        /// option.
        /// </summary>
        /// <param name="key">The option to find a parsed option for.</param>
        /// <param name="value">The parsed option for the specified option.</param>
        /// <returns>
        /// <c>true</c> if a parsed option is found that corresponds to the
        /// option; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetParsedOption(Option key, out ParsedOption value)
        {
            return contents.TryGetValue(key, out value);
        }

        /// <summary>
        /// Looks for the value that has been parsed for a particular
        /// option. If there is no such value, then the default
        /// value for that option is produced.
        /// </summary>
        /// <param name="opt">An option to find a parsed value for.</param>
        /// <param name="result">
        /// The variable in which the resulting value is stored.
        /// </param>
        /// <returns>
        /// <c>true</c> if a form of the option has been parsed;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue<T>(Option opt, out T result)
        {
            ParsedOption parsedOpt;
            if (contents.TryGetValue(opt, out parsedOpt))
            {
                result = (T)parsedOpt.Value;
                return true;
            }
            else
            {
                result = (T)opt.DefaultValue;
                return false;
            }
        }

        /// <summary>
        /// Looks for the Boolean value that has been parsed for a flag option.
        /// If there is no such value, then the flag's default value is produced.
        /// </summary>
        /// <param name="opt">A flag option to find a parsed value for.</param>
        /// <param name="result">The resulting Boolean value.</param>
        /// <returns>
        /// <c>true</c> if a form of the flag has been parsed;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(FlagOption opt, out bool result)
        {
            return TryGetValue<bool>(opt, out result);
        }

        /// <summary>
        /// Looks for the value that has been parsed for a typed value option.
        /// If there is no such value, then the option's default value is produced.
        /// </summary>
        /// <typeparam name="T">The option's value type.</typeparam>
        /// <param name="opt">A typed value option to find a parsed value for.</param>
        /// <param name="result">The resulting value.</param>
        /// <returns>
        /// <c>true</c> if a form of the option has been parsed;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue<T>(ValueOption<T> opt, out T result)
        {
            return TryGetValue<T>((Option)opt, out result);
        }

        /// <summary>
        /// Looks for the values that have been parsed for a typed sequence option.
        /// If there is no such value, then the option's default value is produced.
        /// </summary>
        /// <typeparam name="T">The option's element type.</typeparam>
        /// <param name="opt">A typed sequence option to find parsed values for.</param>
        /// <param name="result">The resulting sequence of values.</param>
        /// <returns>
        /// <c>true</c> if a form of the option has been parsed;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue<T>(SequenceOption<T> opt, out IReadOnlyList<T> result)
        {
            return TryGetValue<IReadOnlyList<T>>((Option)opt, out result);
        }

        /// <summary>
        /// Gets the value that has been parsed for a particular
        /// option. If there is no such value, then the default
        /// value for that option is returned.
        /// </summary>
        /// <remarks>
        /// This method does not tell you whether the option was explicitly
        /// present on the command line. Use <see cref="TryGetValue{T}(Option, out T)"/>
        /// or <see cref="ContainsOption(Option)"/> when that distinction matters.
        /// </remarks>
        /// <param name="opt">An option to find a parsed value for.</param>
        /// <returns>
        /// The option's value if a form of the option has been parsed;
        /// otherwise, the option's default value
        /// </returns>
        public T GetValue<T>(Option opt)
        {
            T result;
            TryGetValue<T>(opt, out result);
            return result;
        }

        /// <summary>
        /// Gets the Boolean value that has been parsed for a flag option.
        /// If there is no such value, then the flag's default value is returned.
        /// </summary>
        /// <param name="opt">A flag option to find a parsed value for.</param>
        /// <returns>The parsed value or the flag's default value.</returns>
        public bool GetValue(FlagOption opt)
        {
            return GetValue<bool>(opt);
        }

        /// <summary>
        /// Gets the value that has been parsed for a typed value option.
        /// If there is no such value, then the option's default value is returned.
        /// </summary>
        /// <typeparam name="T">The option's value type.</typeparam>
        /// <param name="opt">A typed value option to find a parsed value for.</param>
        /// <returns>The parsed value or the option's default value.</returns>
        public T GetValue<T>(ValueOption<T> opt)
        {
            return GetValue<T>((Option)opt);
        }

        /// <summary>
        /// Gets the values that have been parsed for a typed sequence option.
        /// If there is no such value, then the option's default value is returned.
        /// </summary>
        /// <typeparam name="T">The option's element type.</typeparam>
        /// <param name="opt">A typed sequence option to find parsed values for.</param>
        /// <returns>The parsed values or the option's default value.</returns>
        public IReadOnlyList<T> GetValue<T>(SequenceOption<T> opt)
        {
            return GetValue<IReadOnlyList<T>>((Option)opt);
        }
    }

    /// <summary>
    /// Defines a common interface for option set parsers.
    /// </summary>
    public abstract class OptionSetParser
    {
        /// <summary>
        /// Parses an option set from a list of command-line arguments.
        /// </summary>
        /// <param name="arguments">A list of command-line arguments.</param>
        /// <param name="log">
        /// A log to which messages can be sent as arguments are parsed.
        /// </param>
        /// <returns>A parsed option set.</returns>
        public abstract OptionSet Parse(
            IReadOnlyList<string> arguments,
            ILog log);
    }
}
