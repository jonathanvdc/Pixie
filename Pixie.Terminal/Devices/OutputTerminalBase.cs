using System;

namespace Pixie.Terminal
{
    /// <summary>
    /// A base class for end-of-the line terminals that produce output.
    /// It helps build separators properly.
    /// </summary>
    public abstract class OutputTerminalBase : TerminalBase
    {
        private int sepLineCounter;

        protected abstract void WriteLineImpl();

        /// <inheritdoc/>
        public sealed override void WriteLine()
        {
            WriteSeparator(1);
        }

        /// <inheritdoc/>
        public override void WriteSeparator(int lineCount)
        {
            int oldSepLineCounter = sepLineCounter;
            sepLineCounter = Math.Max(lineCount, sepLineCounter);
            for (int i = oldSepLineCounter; i < sepLineCounter; i++)
            {
                WriteLineImpl();
            }
        }

        protected void EndSeparator()
        {
            sepLineCounter = 0;
        }
    }
}