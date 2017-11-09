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
                new Text("This is a hello world example")));
        }
    }
}
