using System;

namespace Pixie.Terminal
{
    /// <summary>
    /// A log implement that logs messages to standard output.
    /// </summary>
    public sealed class TerminalLog : ILog
    {
        public TerminalLog()
        {
        }

        public void Log(LogEntry entry)
        {
            Console.WriteLine();
        }
    }
}