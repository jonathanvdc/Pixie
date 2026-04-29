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

        public TerminalBase Terminal { get; private set; }

        public void Write(Block block)
        {
            lock (renderLock)
            {
                renderer.Render(compiler.CompileBlock(block), Terminal);
                Terminal.WriteLine();
            }
        }

        public void Write(Inline inline)
        {
            Write(new Paragraph(inline));
        }

        public static TerminalSession Acquire()
        {
            return AcquireStandardError();
        }

        public static TerminalSession Acquire(TerminalBase terminal)
        {
            return new TerminalSession(terminal);
        }

        public static TerminalSession AcquireStandardError()
        {
            return Acquire(TextWriterTerminal.FromErrorStream());
        }

        public static TerminalSession AcquireStandardError(StyleManager styleManager)
        {
            return Acquire(TextWriterTerminal.FromErrorStream(styleManager));
        }

        public static TerminalSession AcquireStandardOutput()
        {
            return Acquire(TextWriterTerminal.FromOutputStream());
        }

        public static TerminalSession AcquireStandardOutput(StyleManager styleManager)
        {
            return Acquire(TextWriterTerminal.FromOutputStream(styleManager));
        }
    }
}
