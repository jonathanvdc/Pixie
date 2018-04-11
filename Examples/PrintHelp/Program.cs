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

            // Create the help message itself.
            return new Sequence(
                new MarkupNode[]
                {
                    // Let's start off with a summary of all options.

                    // We'll put the title of this section in bold.
                    DecorationSpan.MakeBold("Option summary"),
                    // And also indent + word-wrap the contents.
                    WrapBox.IndentAndWordWrap(
                        new MarkupNode[]
                        {
                            // Short introductory paragraph.
                            new Paragraph(
                                "Here is a summary of all the options, grouped by type. " +
                                "Explanations are in the following sections."),

                            // Actually summarize the options.
                            new OptionSummary(allOptions, GnuOptionPrinter.Instance)
                        }),
                });
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
