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
                    ColorSpanRenderer.Instance,
                    DecorationSpanRenderer.Instance,
                    DegradableTextRenderer.Instance,
                    DiagnosticRenderer.Instance,
                    new HighlightedSourceRenderer(1, Colors.Green),
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
            lock (renderLock)
            {
                BaseRenderState.Render(new Paragraph(entry.Contents));
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
        /// </summary>
        /// <returns>A terminal log.</returns>
        public static TerminalLog Acquire()
        {
            var writerTerm = TextWriterTerminal.FromErrorStream();
            return new TerminalLog(LayoutTerminal.Wrap(writerTerm, WrappingStrategy.Character));
        }
    }
}