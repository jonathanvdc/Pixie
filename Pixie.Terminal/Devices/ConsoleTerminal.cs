using System;
using System.Collections.Generic;
using System.IO;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that uses the 'System.Console' class directly.
    /// </summary>
    public sealed class ConsoleTerminal : TextWriterTerminal
    {
        public ConsoleTerminal()
            : base(
                Console.Error,
                Console.WindowWidth,
                new ConsoleStyleManager())
        { }
    }
}