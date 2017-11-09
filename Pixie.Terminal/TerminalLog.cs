using System;
using System.Threading;
using Pixie.Terminal.Devices;
using Pixie.Terminal.Render;

namespace Pixie.Terminal
{
    /// <summary>
    /// A log implement that logs messages to standard output.
    /// </summary>
    public sealed class TerminalLog : ILog
    {
        /// <summary>
        /// Creates a terminal log from the given terminal.
        /// </summary>
        /// <param name="terminal">A terminal handle.</param>
        public TerminalLog(TerminalBase terminal)
            : this(
                new RenderState(terminal).WithRenderers(
                    NewLineRenderer.Instance,
                    SequenceRenderer.Instance,
                    TextRenderer.Instance))
        { }

        /// <summary>
        /// Creates a terminal log from the given base render state.
        /// </summary>
        /// <param name="baseRenderState">A base render state.</param>
        public TerminalLog(RenderState baseRenderState)
        {
            this.BaseRenderState = baseRenderState;
            this.renderLock = new object();
        }

        /// <summary>
        /// Gets the base render state for this terminal log.
        /// </summary>
        /// <returns>The base render state.</returns>
        public RenderState BaseRenderState { get; private set; }

        private object renderLock;

        /// <summary>
        /// Logs the given entry in this log.
        /// </summary>
        /// <param name="entry">The entry to log.</param>
        public void Log(LogEntry entry)
        {
            lock (renderLock)
            {
                BaseRenderState.Render(entry.Contents);
            }
        }

        /// <summary>
        /// Acquires a terminal log for the current environment.
        /// </summary>
        /// <returns>A terminal log.</returns>
        public static TerminalLog Acquire()
        {
            return new TerminalLog(new ConsoleTerminal());
        }
    }
}