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
        private bool hasWrittenContent;

        /// <summary>
        /// Writes a raw newline to the output.
        /// </summary>
        protected abstract void WriteLineImpl();

        /// <summary>
        /// Marks that visible output content has been written.
        /// </summary>
        protected void NoteWrittenContent()
        {
            hasWrittenContent = true;
        }

        /// <inheritdoc/>
        public override bool HasWrittenContent => hasWrittenContent;

        private void FlushSeparator()
        {
            for (int i = 0; i < sepLineCounter; i++)
            {
                WriteLineImpl();
            }
            sepLineCounter = 0;
        }

        /// <inheritdoc/>
        public sealed override void WriteLine()
        {
            if (HasWrittenContent)
            {
                WriteSeparator(1);
                FlushSeparator();
            }
        }

        /// <inheritdoc/>
        public override void WriteSeparator(int lineCount)
        {
            if (!hasWrittenContent)
            {
                return;
            }

            sepLineCounter = Math.Max(lineCount, sepLineCounter);
        }

        /// <summary>
        /// Hints that the end of a separator has been reached.
        /// </summary>
        protected void EndSeparator()
        {
            FlushSeparator();
        }

        /// <inheritdoc/>
        public override void FinishOutput()
        {
            if (HasWrittenContent)
            {
                FlushSeparator();
            }
        }
    }
}
