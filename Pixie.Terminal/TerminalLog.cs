using Pixie.Markup;

namespace Pixie.Terminal
{
    /// <summary>
    /// A log implementation that writes entries through a terminal session.
    /// </summary>
    public sealed class TerminalLog : ILog
    {
        public TerminalLog(TerminalBase terminal)
            : this(new TerminalSession(terminal))
        { }

        public TerminalLog(TerminalSession session)
        {
            this.Session = session;
        }

        public TerminalSession Session { get; private set; }

        public void Log(LogEntry entry)
        {
            Session.Write(entry.Contents);
        }

        public void Log(Block node)
        {
            Session.Write(node);
        }

        public static TerminalLog Acquire()
        {
            return AcquireStandardError();
        }

        public static TerminalLog Acquire(TerminalBase terminal)
        {
            return new TerminalLog(TerminalSession.Acquire(terminal));
        }

        public static TerminalLog AcquireStandardError()
        {
            return new TerminalLog(TerminalSession.AcquireStandardError());
        }

        public static TerminalLog AcquireStandardError(StyleManager styleManager)
        {
            return new TerminalLog(TerminalSession.AcquireStandardError(styleManager));
        }

        public static TerminalLog AcquireStandardOutput()
        {
            return new TerminalLog(TerminalSession.AcquireStandardOutput());
        }

        public static TerminalLog AcquireStandardOutput(StyleManager styleManager)
        {
            return new TerminalLog(TerminalSession.AcquireStandardOutput(styleManager));
        }
    }
}
