namespace Pixie.Terminal.Render
{
    /// <summary>
    /// Captures the current state of the renderer.
    /// </summary>
    public sealed class RenderState
    {
        /// <summary>
        /// Creates a render state from the given terminal handle.
        /// </summary>
        /// <param name="terminal">A terminal handle.</param>
        public RenderState(Terminal terminal)
        {
            this.Terminal = terminal;
        }

        /// <summary>
        /// Gets the terminal to render to.
        /// </summary>
        /// <returns>The terminal to render to.</returns>
        public Terminal Terminal { get; private set; }
    }
}