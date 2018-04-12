using System;
using Pixie.Terminal;
using Pixie.Markup;
using Pixie;

namespace SimpleErrorMessage
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // This example demonstrates how to log a super
            // simple error diagnostic.

            // First acquire a log. You should do this only
            // once over the course of your program.
            var log = TerminalLog.Acquire();

            // Compose an error message. We'll quote `Color.exe`
            // and `Main` and put them in bold.
            //
            // Note that the array of MarkupNodes below contains
            // only strings. That's fine because strings are
            // implicitly convertible to text MarkupNodes.
            var message = Quotation.QuoteEvenInBold(
                new MarkupNode[]
                {
                    "program ",
                    "Color.exe",
                    " does not contain a static ",
                    "Main",
                    " method suitable for an entry point."
                });

            // Log an error diagnostic.
            log.Log(
                new LogEntry(
                    Severity.Error,
                    WrapBox.WordWrap(
                        new Diagnostic(
                            "mcs",
                            "error",
                            Colors.Red,
                            "CS5001",
                            message))));
        }
    }
}
