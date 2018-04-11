using System;
using Pixie.Options;
using Pixie;
using Pixie.Terminal;
using Pixie.Markup;

namespace PrintHelp
{
    public static class Program
    {
        /// <summary>
        /// Composes the help message itself.
        /// </summary>
        /// <returns>The help message.</returns>
        public static MarkupNode ComposeHelpMessage()
        {
            // This is the gist of this example.

            // Create a list of all options our application accepts.
            var allOptions = new Option[]
            {
                filesOption,
                syntaxOnlyFlag,
                optimizeOption,
                optimizeFastFlag,
                helpFlag
            };

            return new HelpMessage(
                "PrintHelp is an example program that showcases " +
                "how easy it is to create pretty help messages using Pixie.",
                "PrintHelp [files-or-options]",
                allOptions);
        }

        public static void Main(string[] args)
        {
            // First, acquire a terminal log. You should acquire
            // a log once and then re-use it in your application.
            var log = TerminalLog.Acquire();

            // Wrap the help message into a log entry and send it to the log.
            log.Log(
                new LogEntry(
                    Severity.Info,
                    ComposeHelpMessage()));
        }

        // A number of option definitions.

        // This option is a simple flag. It takes no arguments.
        private static readonly FlagOption optimizeFastFlag =
            new FlagOption(OptionForm.Short("Ofast"))
                .WithCategory("Optimization options")
                .WithDescription("Enable aggressive optimizations.");

        // This option is also a simple flag, but it has two
        // different forms:
        //     -h and --help.
        private static readonly FlagOption helpFlag =
            FlagOption.CreateFlagOption(
                new OptionForm[]
                {
                    OptionForm.Short("h"),
                    OptionForm.Long("help")
                })
                .WithDescription(
                    "Print a description of the options understood.");

        // This option has both a positive and a negative form:
        //     -fsyntax-only and -fno-syntax-only.
        private static readonly FlagOption syntaxOnlyFlag =
            new FlagOption(
                OptionForm.Short("fsyntax-only"),
                OptionForm.Short("fno-syntax-only"),
                false)
                .WithDescription("Check the code for syntax errors only.");

        // This option takes zero or more strings as arguments.
        private static readonly SequenceOption<string> filesOption =
            SequenceOption.CreateStringOption(
                OptionForm.Long("files"))
                .WithDescription("Consume files as input.")
                .WithParameters(
                    new OptionParameter[]
                    {
                        new SymbolicOptionParameter("file", true)
                    });

        // This option takes a 32-bit signed integer as argument.
        private static readonly ValueOption<int> optimizeOption =
            ValueOption.CreateInt32Option(
                OptionForm.Short("O"),
                0)
                .WithCategory("Optimization options")
                .WithDescription("Pick an optimization level.")
                .WithParameter(new SymbolicOptionParameter("n"));
    }
}
