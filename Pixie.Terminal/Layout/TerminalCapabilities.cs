namespace Pixie.Terminal.Layout
{
    internal sealed class TerminalCapabilities
    {
        public TerminalCapabilities(TerminalBase terminal)
        {
            Terminal = terminal;
        }

        public TerminalBase Terminal { get; private set; }

        public bool CanRender(string text)
        {
            return Terminal.CanRender(text);
        }
    }
}
