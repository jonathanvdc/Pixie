using System;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that uses the 'System.Console' class directly.
    /// </summary>
    public sealed class ConsoleTerminal : TerminalBase
    {
        public ConsoleTerminal()
        {
        }

        /// <inheritdoc/>
        public override void Write(string text)
        {
            Console.Write(text);
        }
    }
}