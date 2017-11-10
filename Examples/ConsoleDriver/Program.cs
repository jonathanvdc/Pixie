using System;
using Pixie;
using Pixie.Terminal;
using Pixie.Markup;

namespace ConsoleDriver
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var log = TerminalLog.Acquire();
            log.Log(new LogEntry(
                Severity.Progress,
                new Title("Hello world"),
                new Box(
                    new Text(
                        "This is a hello world example. Wow this is a really long piece of text!!! " +
                        "I wonder if this'll fit on a single line."),
                    WrappingStrategy.Word,
                    4)));
        }
    }
}
