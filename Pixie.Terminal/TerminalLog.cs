using System;
using System.Threading;
using Pixie.Markup;
using Pixie.Terminal.Devices;
using Pixie.Terminal.Render;

namespace Pixie.Terminal
{
    /// <summary>
    /// A log implementation that logs messages to a terminal.
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
                    AlignBoxRenderer.Instance,
                    BoxRenderer.Instance,
                    ColorSpanRenderer.Instance,
                    DecorationSpanRenderer.Instance,
                    DegradableTextRenderer.Instance,
                    DiagnosticRenderer.Instance,
                    new HighlightedSourceRenderer(1, Colors.Green),
                    IndentBoxRenderer.Instance,
                    NewLineRenderer.Instance,
                    ParagraphRenderer.Instance,
                    PrefixBoxRenderer.Instance,
                    SequenceRenderer.Instance,
                    TextRenderer.Instance,
                    WrapBoxRenderer.Instance))
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
            Log(entry.Contents);
        }

        /// <summary>
        /// Writes a markup node to this log directly.
        /// </summary>
        /// <param name="node">The markup node to write.</param>
        public void Log(MarkupNode node)
        {
            lock (renderLock)
            {
                BaseRenderState.Render(new Box(node));
            }
        }

        /// <summary>
        /// Creates a new terminal log that has an additional set of renderers.
        /// </summary>
        /// <param name="extraRenderers">A sequence of additional node renderers.</param>
        /// <returns>A new terminal log.</returns>
        public TerminalLog WithRenderers(params NodeRenderer[] extraRenderers)
        {
            return new TerminalLog(BaseRenderState.WithRenderers(extraRenderers));
        }

        /// <summary>
        /// Acquires a terminal log for the current environment.
        /// The terminal's output is sent to standard error.
        /// </summary>
        /// <returns>A terminal log.</returns>
        public static TerminalLog Acquire()
        {
            return AcquireStandardError();
        }

        /// <summary>
        /// Creates a terminal log from a terminal.
        /// </summary>
        /// <param name="terminal">
        /// The terminal to send output to.
        /// </param>
        /// <returns>A terminal log.</returns>
        public static TerminalLog Acquire(TerminalBase terminal)
        {
            return new TerminalLog(LayoutTerminal.Wrap(terminal, WrappingStrategy.Character));
        }

        /// <summary>
        /// Acquires a terminal log for the current environment.
        /// The terminal's output is sent to standard error.
        /// </summary>
        /// <returns>A terminal log.</returns>
        public static TerminalLog AcquireStandardError()
        {
            return Acquire(TextWriterTerminal.FromErrorStream());
        }

        /// <summary>
        /// Acquires a terminal log for the current environment
        /// and a particular style manager.
        /// The terminal's output is sent to standard error.
        /// </summary>
        /// <param name="styleManager">
        /// A style manager to style the terminal's output with.
        /// </param>
        /// <returns>A terminal log.</returns>
        public static TerminalLog AcquireStandardError(
            StyleManager styleManager)
        {
            return Acquire(TextWriterTerminal.FromErrorStream(styleManager));
        }

        /// <summary>
        /// Acquires a terminal log for the current environment.
        /// The terminal's output is sent to standard output.
        /// </summary>
        /// <returns>A terminal log.</returns>
        public static TerminalLog AcquireStandardOutput()
        {
            return Acquire(TextWriterTerminal.FromOutputStream());
        }

        /// <summary>
        /// Acquires a terminal log for the current environment
        /// and a particular style manager.
        /// The terminal's output is sent to standard output.
        /// </summary>
        /// <param name="styleManager">
        /// A style manager to style the terminal's output with.
        /// </param>
        /// <returns>A terminal log.</returns>
        public static TerminalLog AcquireStandardOutput(
            StyleManager styleManager)
        {
            return Acquire(TextWriterTerminal.FromOutputStream(styleManager));
        }
    }
}
