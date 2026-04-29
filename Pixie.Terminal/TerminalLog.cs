using Pixie.Markup;

namespace Pixie.Terminal
{
    /// <summary>
    /// A log implementation that writes entries through a terminal session.
    /// </summary>
    public sealed class TerminalLog : ILog
    {
        /// <summary>
        /// Creates a terminal log from a terminal device.
        /// </summary>
        /// <param name="terminal">The terminal device to write to.</param>
        public TerminalLog(TerminalBase terminal)
            : this(new TerminalSession(terminal))
        { }

        /// <summary>
        /// Creates a terminal log from a terminal session.
        /// </summary>
        /// <param name="session">The terminal session to use.</param>
        public TerminalLog(TerminalSession session)
        {
            this.Session = session;
        }

        /// <summary>
        /// Gets the terminal session used by this log.
        /// </summary>
        public TerminalSession Session { get; private set; }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        public void Log(LogEntry entry)
        {
            Session.Write(entry.Contents);
        }

        /// <summary>
        /// Writes a block of markup.
        /// </summary>
        /// <param name="node">The block markup to write.</param>
        public void Log(Block node)
        {
            Session.Write(node);
        }

        /// <summary>
        /// Acquires a terminal log that writes to standard error.
        /// </summary>
        /// <returns>A terminal log bound to standard error.</returns>
        public static TerminalLog Acquire()
        {
            return AcquireStandardError();
        }

        /// <summary>
        /// Acquires a terminal log for a terminal device.
        /// </summary>
        /// <param name="terminal">The terminal device to write to.</param>
        /// <returns>A terminal log bound to the terminal device.</returns>
        public static TerminalLog Acquire(TerminalBase terminal)
        {
            return new TerminalLog(TerminalSession.Acquire(terminal));
        }

        /// <summary>
        /// Acquires a terminal log that writes to standard error.
        /// </summary>
        /// <returns>A terminal log bound to standard error.</returns>
        public static TerminalLog AcquireStandardError()
        {
            return new TerminalLog(TerminalSession.AcquireStandardError());
        }

        /// <summary>
        /// Acquires a styled terminal log that writes to standard error.
        /// </summary>
        /// <param name="styleManager">The style manager to use.</param>
        /// <returns>A terminal log bound to standard error.</returns>
        public static TerminalLog AcquireStandardError(StyleManager styleManager)
        {
            return new TerminalLog(TerminalSession.AcquireStandardError(styleManager));
        }

        /// <summary>
        /// Acquires a terminal log that writes to standard output.
        /// </summary>
        /// <returns>A terminal log bound to standard output.</returns>
        public static TerminalLog AcquireStandardOutput()
        {
            return new TerminalLog(TerminalSession.AcquireStandardOutput());
        }

        /// <summary>
        /// Acquires a styled terminal log that writes to standard output.
        /// </summary>
        /// <param name="styleManager">The style manager to use.</param>
        /// <returns>A terminal log bound to standard output.</returns>
        public static TerminalLog AcquireStandardOutput(StyleManager styleManager)
        {
            return new TerminalLog(TerminalSession.AcquireStandardOutput(styleManager));
        }
    }
}
