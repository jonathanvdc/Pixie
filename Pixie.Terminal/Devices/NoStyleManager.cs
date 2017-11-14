namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A style manager implementation that does not apply any styles.
    /// </summary>
    public sealed class NoStyleManager : StyleManager
    {
        private NoStyleManager()
        { }

        /// <summary>
        /// A style manager instance that does not apply any styles.
        /// </summary>
        /// <returns>A style manager instance.</returns>
        public static readonly NoStyleManager Instance = new NoStyleManager();

        /// <inheritdoc/>
        public override void PopStyle()
        {

        }

        /// <inheritdoc/>
        public override void PushBackgroundColor(Color color)
        {

        }

        /// <inheritdoc/>
        public override void PushForegroundColor(Color color)
        {

        }
    }
}