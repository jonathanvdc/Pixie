using System.Collections.Generic;

namespace Pixie.Options
{
    /// <summary>
    /// Represents the outcome of parsing a command line.
    /// </summary>
    public sealed class OptionParseResult
    {
        /// <summary>
        /// Creates a parse result from parsed options and recorded diagnostics.
        /// </summary>
        /// <param name="options">The parsed options.</param>
        /// <param name="diagnostics">The diagnostics emitted while parsing.</param>
        /// <param name="wasHandled">
        /// Tells if parsing already handled a conventional exit path, such as
        /// printing generated help or version information.
        /// </param>
        /// <param name="exitCode">The recommended process exit code.</param>
        /// <param name="wasHelpRequested">
        /// Tells if the help option was requested.
        /// </param>
        /// <param name="wasVersionRequested">
        /// Tells if the version option was requested.
        /// </param>
        public OptionParseResult(
            OptionSet options,
            IReadOnlyList<LogEntry> diagnostics,
            bool wasHandled,
            int exitCode,
            bool wasHelpRequested,
            bool wasVersionRequested)
        {
            this.Options = options;
            this.Diagnostics = diagnostics;
            this.WasHandled = wasHandled;
            this.ExitCode = exitCode;
            this.WasHelpRequested = wasHelpRequested;
            this.WasVersionRequested = wasVersionRequested;
        }

        /// <summary>
        /// Gets the parsed options.
        /// </summary>
        /// <returns>The parsed options.</returns>
        public OptionSet Options { get; private set; }

        /// <summary>
        /// Gets the diagnostics emitted while parsing.
        /// </summary>
        /// <returns>The diagnostics emitted while parsing.</returns>
        public IReadOnlyList<LogEntry> Diagnostics { get; private set; }

        /// <summary>
        /// Tells if parsing already handled a conventional exit path,
        /// such as printing generated help or version information.
        /// This is typically <c>true</c> when <see cref="WasHelpRequested"/>
        /// or <see cref="WasVersionRequested"/> is <c>true</c>.
        /// It is typically <c>false</c> for ordinary successful parses,
        /// where the caller should continue running the command, and also
        /// for parse failures, where <see cref="IsSuccess"/> is <c>false</c>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if parsing printed a built-in terminal response and the
        /// caller will usually want to return immediately; otherwise, <c>false</c>.
        /// </returns>
        public bool WasHandled { get; private set; }

        /// <summary>
        /// Gets the exit code associated with this parse result.
        /// </summary>
        /// <returns>The recommended exit code.</returns>
        public int ExitCode { get; private set; }

        /// <summary>
        /// Tells if the help option was requested.
        /// </summary>
        /// <returns>
        /// <c>true</c> if help was requested; otherwise, <c>false</c>.
        /// </returns>
        public bool WasHelpRequested { get; private set; }

        /// <summary>
        /// Tells if the version option was requested.
        /// </summary>
        /// <returns>
        /// <c>true</c> if version information was requested; otherwise, <c>false</c>.
        /// </returns>
        public bool WasVersionRequested { get; private set; }

        /// <summary>
        /// Tells if parsing succeeded without errors.
        /// </summary>
        /// <returns>
        /// <c>true</c> if no errors were emitted while parsing;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsSuccess => !Contains(Severity.Error);

        /// <summary>
        /// Tests if parsing emitted at least one diagnostic of a particular severity.
        /// </summary>
        /// <param name="severity">The severity to look for.</param>
        /// <returns>
        /// <c>true</c> if parsing emitted a diagnostic of the requested severity;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Severity severity)
        {
            int count = Diagnostics.Count;
            for (int i = 0; i < count; i++)
            {
                if (Diagnostics[i].Severity == severity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Looks for the value that has been parsed for a particular option.
        /// If there is no such value, then the option's default value is produced.
        /// </summary>
        /// <param name="opt">An option to find a parsed value for.</param>
        /// <param name="result">The resulting value.</param>
        /// <typeparam name="T">The option's value type.</typeparam>
        /// <returns>
        /// <c>true</c> if a form of the option has been parsed;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue<T>(Option opt, out T result)
        {
            return Options.TryGetValue<T>(opt, out result);
        }

        /// <summary>
        /// Gets the value that has been parsed for a particular option.
        /// If there is no such value, then the option's default value is returned.
        /// </summary>
        /// <param name="opt">An option to find a parsed value for.</param>
        /// <typeparam name="T">The option's value type.</typeparam>
        /// <returns>The parsed value or the option's default value.</returns>
        public T GetValue<T>(Option opt)
        {
            return Options.GetValue<T>(opt);
        }
    }
}
