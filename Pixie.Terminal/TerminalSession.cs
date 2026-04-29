using Pixie.Markup;
using Pixie.Terminal.Devices;
using Pixie.Terminal.Layout;

namespace Pixie.Terminal
{
    /// <summary>
    /// Owns terminal rendering for append-only Pixie output.
    /// </summary>
    public sealed class TerminalSession
    {
        /// <summary>
        /// Creates a terminal session.
        /// </summary>
        /// <param name="terminal">The terminal device to render to.</param>
        public TerminalSession(TerminalBase terminal)
        {
            this.Terminal = terminal;
            this.compiler = new LayoutCompiler();
            this.renderer = new LayoutRenderer();
            this.renderLock = new object();
        }

        private readonly LayoutCompiler compiler;
        private readonly LayoutRenderer renderer;
        private readonly object renderLock;

        /// <summary>
        /// Gets the terminal device used by this session.
        /// </summary>
        public TerminalBase Terminal { get; private set; }

        /// <summary>
        /// Renders a block of markup to the terminal.
        /// </summary>
        /// <param name="block">The block markup to render.</param>
        public void Write(Block block)
        {
            lock (renderLock)
            {
                renderer.Render(compiler.CompileBlock(block), Terminal);
                Terminal.WriteLine();
            }
        }

        /// <summary>
        /// Renders inline markup to the terminal.
        /// </summary>
        /// <param name="inline">The inline markup to render.</param>
        public void Write(Inline inline)
        {
            Write(new Paragraph(inline));
        }

        /// <summary>
        /// Acquires a terminal session that writes to standard error.
        /// </summary>
        /// <returns>A terminal session bound to standard error.</returns>
        public static TerminalSession Acquire()
        {
            return AcquireStandardError();
        }

        /// <summary>
        /// Acquires a terminal session for a terminal device.
        /// </summary>
        /// <param name="terminal">The terminal device to render to.</param>
        /// <returns>A terminal session bound to the terminal device.</returns>
        public static TerminalSession Acquire(TerminalBase terminal)
        {
            return new TerminalSession(terminal);
        }

        /// <summary>
        /// Acquires a terminal session that writes to standard error.
        /// </summary>
        /// <returns>A terminal session bound to standard error.</returns>
        public static TerminalSession AcquireStandardError()
        {
            return Acquire(TextWriterTerminal.FromErrorStream());
        }

        /// <summary>
        /// Acquires a styled terminal session that writes to standard error.
        /// </summary>
        /// <param name="styleManager">The style manager to use.</param>
        /// <returns>A terminal session bound to standard error.</returns>
        public static TerminalSession AcquireStandardError(StyleManager styleManager)
        {
            return Acquire(TextWriterTerminal.FromErrorStream(styleManager));
        }

        /// <summary>
        /// Acquires a terminal session that writes to standard output.
        /// </summary>
        /// <returns>A terminal session bound to standard output.</returns>
        public static TerminalSession AcquireStandardOutput()
        {
            return Acquire(TextWriterTerminal.FromOutputStream());
        }

        /// <summary>
        /// Acquires a styled terminal session that writes to standard output.
        /// </summary>
        /// <param name="styleManager">The style manager to use.</param>
        /// <returns>A terminal session bound to standard output.</returns>
        public static TerminalSession AcquireStandardOutput(StyleManager styleManager)
        {
            return Acquire(TextWriterTerminal.FromOutputStream(styleManager));
        }
    }
}
