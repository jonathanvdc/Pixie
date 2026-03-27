using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Options
{
    /// <summary>
    /// Defines the options accepted by a command-line program and parses
    /// argument lists into an <see cref="OptionParseResult"/>.
    /// Use this type when you want one place to describe your options,
    /// positional arguments, and common built-in behaviors such as
    /// <c>--help</c> and <c>--version</c>.
    /// </summary>
    public sealed class CommandLine
    {
        /// <summary>
        /// Creates a command line that accepts only named options.
        /// </summary>
        /// <param name="options">The options accepted by the command line.</param>
        public CommandLine(params Option[] options)
            : this(
                options,
                new KeyValuePair<Option, OptionForm>[0])
        { }

        /// <summary>
        /// Creates a command line with one option that also consumes
        /// positional arguments.
        /// </summary>
        /// <param name="options">The options accepted by the command line.</param>
        /// <param name="positionalOption">
        /// The option that should consume bare arguments such as file names.
        /// </param>
        public CommandLine(
            IReadOnlyList<Option> options,
            Option positionalOption)
            : this(
                options,
                positionalOption,
                positionalOption.Forms[0])
        { }

        /// <summary>
        /// Creates a command line with one option that also consumes
        /// positional arguments, using a specific option form when Pixie
        /// needs to describe that positional usage.
        /// </summary>
        /// <param name="options">The options accepted by the command line.</param>
        /// <param name="positionalOption">
        /// The option that should consume bare arguments such as file names.
        /// </param>
        /// <param name="positionalForm">
        /// The preferred form for the positional option in help and usage text.
        /// </param>
        public CommandLine(
            IReadOnlyList<Option> options,
            Option positionalOption,
            OptionForm positionalForm)
            : this(
                options,
                new[]
                {
                    new KeyValuePair<Option, OptionForm>(positionalOption, positionalForm)
                })
        { }

        /// <summary>
        /// Creates a command line with explicitly configured positional bindings.
        /// </summary>
        /// <param name="options">The options accepted by the command line.</param>
        /// <param name="positionalOptions">
        /// The options that should consume bare arguments, in the order they
        /// should be considered.
        /// </param>
        public CommandLine(
            IReadOnlyList<Option> options,
            IReadOnlyList<KeyValuePair<Option, OptionForm>> positionalOptions)
        {
            this.Options = options;
            this.PositionalOptions = positionalOptions;
        }

        private CommandLine(CommandLine other)
        {
            this.Options = other.Options;
            this.PositionalOptions = other.PositionalOptions;
            this.Summary = other.Summary;
            this.Usage = other.Usage;
            this.Version = other.Version;
            this.HelpOption = other.HelpOption;
            this.VersionOption = other.VersionOption;
        }

        /// <summary>
        /// Gets the options accepted by this command line.
        /// </summary>
        /// <returns>The accepted options.</returns>
        public IReadOnlyList<Option> Options { get; private set; }

        /// <summary>
        /// Gets the positional option bindings for this command line.
        /// </summary>
        /// <returns>The positional option bindings.</returns>
        public IReadOnlyList<KeyValuePair<Option, OptionForm>> PositionalOptions { get; private set; }

        /// <summary>
        /// Gets the summary used for generated help output.
        /// </summary>
        /// <returns>The summary.</returns>
        public MarkupNode Summary { get; private set; }

        /// <summary>
        /// Gets the usage text used for generated help output.
        /// </summary>
        /// <returns>The usage text.</returns>
        public MarkupNode Usage { get; private set; }

        /// <summary>
        /// Gets the version text emitted by the generated version option.
        /// </summary>
        /// <returns>The version text.</returns>
        public MarkupNode Version { get; private set; }

        /// <summary>
        /// Gets the generated help option, if any.
        /// </summary>
        /// <returns>The help option.</returns>
        public Option HelpOption { get; private set; }

        /// <summary>
        /// Gets the generated version option, if any.
        /// </summary>
        /// <returns>The version option.</returns>
        public Option VersionOption { get; private set; }

        /// <summary>
        /// Creates a copy of this command line that also supports a generated
        /// help option, typically <c>-h</c> and <c>--help</c>.
        /// </summary>
        /// <param name="summary">The summary shown in generated help output.</param>
        /// <param name="usage">The usage text shown in generated help output.</param>
        /// <param name="forms">
        /// The forms accepted by the help option. If omitted, Pixie uses
        /// <c>-h</c> and <c>--help</c>.
        /// </param>
        /// <returns>A new command line.</returns>
        public CommandLine WithHelp(
            MarkupNode summary,
            MarkupNode usage,
            params string[] forms)
        {
            var result = new CommandLine(this);
            result.HelpOption = Option.Flag(
                forms.Length == 0
                    ? new[] { "-h", "--help" }
                    : forms);
            result.Summary = summary;
            result.Usage = usage;
            result.Options = AppendOption(Options, result.HelpOption);
            return result;
        }

        /// <summary>
        /// Creates a copy of this command line that also supports a generated
        /// help option, typically <c>-h</c> and <c>--help</c>.
        /// </summary>
        /// <param name="summary">The summary shown in generated help output.</param>
        /// <param name="usage">The usage text shown in generated help output.</param>
        /// <param name="forms">
        /// The forms accepted by the help option. If omitted, Pixie uses
        /// <c>-h</c> and <c>--help</c>.
        /// </param>
        /// <returns>A new command line.</returns>
        public CommandLine WithHelp(
            string summary,
            string usage,
            params string[] forms)
        {
            return WithHelp((MarkupNode)summary, usage, forms);
        }

        /// <summary>
        /// Creates a copy of this command line that also supports a generated
        /// version option, typically <c>--version</c>.
        /// </summary>
        /// <param name="version">The version text to emit.</param>
        /// <param name="forms">
        /// The forms accepted by the version option. If omitted, Pixie uses
        /// <c>--version</c>.
        /// </param>
        /// <returns>A new command line.</returns>
        public CommandLine WithVersion(
            MarkupNode version,
            params string[] forms)
        {
            var result = new CommandLine(this);
            result.VersionOption = Option.Flag(
                forms.Length == 0
                    ? new[] { "--version" }
                    : forms);
            result.Version = version;
            result.Options = AppendOption(Options, result.VersionOption);
            return result;
        }

        /// <summary>
        /// Creates a copy of this command line that also supports a generated
        /// version option, typically <c>--version</c>.
        /// </summary>
        /// <param name="version">The version text to emit.</param>
        /// <param name="forms">
        /// The forms accepted by the version option. If omitted, Pixie uses
        /// <c>--version</c>.
        /// </param>
        /// <returns>A new command line.</returns>
        public CommandLine WithVersion(
            string version,
            params string[] forms)
        {
            return WithVersion((MarkupNode)version, forms);
        }

        /// <summary>
        /// Parses a list of command-line arguments and returns both the parsed
        /// values and any diagnostics Pixie produced while parsing.
        /// Diagnostics are captured in the result and not forwarded anywhere.
        /// </summary>
        /// <param name="arguments">The command-line arguments to parse.</param>
        /// <returns>A parse result.</returns>
        public OptionParseResult Parse(IReadOnlyList<string> arguments)
        {
            return Parse(arguments, NullLog.Instance);
        }

        /// <summary>
        /// Parses a list of command-line arguments, forwarding any diagnostics
        /// to the supplied log while also capturing them in the result.
        /// This is the usual entry point for real commands because it lets Pixie
        /// print parse errors, generated help, or version information immediately
        /// while still giving the caller structured access to the outcome.
        /// </summary>
        /// <param name="arguments">The command-line arguments to parse.</param>
        /// <param name="log">The log to forward diagnostics to.</param>
        /// <returns>A parse result.</returns>
        public OptionParseResult Parse(
            IReadOnlyList<string> arguments,
            ILog log)
        {
            var recordingLog = new RecordingLog(log);
            var parser = new GnuOptionSetParser(Options, PositionalOptions);
            var optionSet = parser.Parse(arguments, recordingLog);
            bool hasErrors = recordingLog.Contains(Severity.Error);
            bool helpRequested =
                !hasErrors
                && HelpOption != null
                && optionSet.GetValue<bool>(HelpOption);
            bool versionRequested =
                !hasErrors
                && !helpRequested
                && VersionOption != null
                && optionSet.GetValue<bool>(VersionOption);

            bool shouldExit = false;
            int exitCode = hasErrors ? 1 : 0;

            if (helpRequested)
            {
                recordingLog.Log(
                    new LogEntry(
                        Severity.Info,
                        new HelpMessage(
                            Summary ?? "",
                            Usage ?? "",
                            Options)));
                shouldExit = true;
            }
            else if (versionRequested)
            {
                recordingLog.Log(new LogEntry(Severity.Info, Version ?? ""));
                shouldExit = true;
            }

            return new OptionParseResult(
                optionSet,
                recordingLog.RecordedEntries,
                shouldExit,
                exitCode,
                helpRequested,
                versionRequested);
        }

        private static IReadOnlyList<Option> AppendOption(
            IReadOnlyList<Option> options,
            Option opt)
        {
            var result = new Option[options.Count + 1];
            for (int i = 0; i < options.Count; i++)
            {
                result[i] = options[i];
            }
            result[options.Count] = opt;
            return result;
        }
    }
}
