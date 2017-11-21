using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Represents a set of parsed options.
    /// </summary>
    public struct ParsedOptionSet
    {
        /// <summary>
        /// Creates a parsed option set from a mapping of
        /// options to parsed options.
        /// </summary>
        /// <param name="contents">A mapping of options to parsed options.</param>
        public ParsedOptionSet(IReadOnlyDictionary<Option, ParsedOption> contents)
        {
            this = default(ParsedOptionSet);
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
        /// Gets the value that has been parsed for a particular
        /// option. If there is no such value, then the default
        /// value for that option is returned.
        /// </summary>
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
    }
}