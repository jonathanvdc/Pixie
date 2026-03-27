namespace Pixie.Terminal.Render
{
    /// <summary>
    /// Common theme property names used by terminal renderers.
    /// </summary>
    public static class RenderThemeProperties
    {
        /// <summary>
        /// When set to <c>true</c>, the first paragraph-like renderer in a
        /// subtree suppresses its leading separator and then clears the flag
        /// for its children.
        /// </summary>
        public const string SuppressLeadingSeparatorProperty =
            "suppress-leading-separator";
    }
}
