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
            var commandLine = new CommandLine(
                filesOption,
                syntaxOnlyFlag,
                optimizeOption,
                optimizeFastFlag)
                .WithHelp(
                    "PrintHelp is an example program that showcases " +
                    "how easy it is to create pretty help messages using Pixie.",
                    "PrintHelp [files-or-options]");

            var parseArgs = args.Length == 0
                ? new[] { "--help" }
                : args;

            var result = commandLine.Parse(parseArgs, log);
            if (!result.IsSuccess || result.WasHandled)
            {
                return;
            }
        }

        // A number of option definitions.

        // This option is a simple flag. It takes no arguments.
        private static readonly FlagOption optimizeFastFlag =
            Option.Flag("-Ofast")
                .WithCategory("Optimization options")
                .WithDescription("Enable aggressive optimizations.");

        // This option is also a simple flag, but it has two
        // different forms:
        //     -h and --help.
        private static readonly FlagOption helpFlag =
            Option.Flag("-h", "--help")
                .WithDescription(
                    "Print a description of the options understood.");

        // This option has both a positive and a negative form:
        //     -fsyntax-only and -fno-syntax-only.
        private static readonly FlagOption syntaxOnlyFlag =
            Option.Toggle(
                "-fsyntax-only",
                "-fno-syntax-only")
                .WithDescription("Check the code for syntax errors only.");

        // This option takes zero or more strings as arguments.
        private static readonly SequenceOption<string> filesOption =
            Option.StringSequence("--files")
                .WithDescription("Consume files as input.")
                .WithParameter("file");

        // This option takes a 32-bit signed integer as argument.
        private static readonly ValueOption<int> optimizeOption =
            Option.Int32WithDefault(
                0,
                "-O")
                .WithCategory("Optimization options")
                .WithDescription("Pick an optimization level.")
                .WithParameter("n");
    }
}
