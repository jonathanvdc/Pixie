using System;
using Pixie.Markup;

namespace Pixie.Terminal
{
    /// <summary>
    /// A base class for objects that manage the style of the text printed
    /// by a terminal.
    /// </summary>
    public abstract class StyleManager
    {
        /// <summary>
        /// Pushes a foreground color onto the style stack.
        /// </summary>
        /// <param name="color">The foreground color to push.</param>
        public abstract void PushForegroundColor(Color color);

        /// <summary>
        /// Pushes a background color onto the style stack.
        /// </summary>
        /// <param name="color">The background color to push.</param>
        public abstract void PushBackgroundColor(Color color);

        /// <summary>
        /// Pushes a text decoration onto the style stack.
        /// </summary>
        /// <param name="decoration">The decoration to apply.</param>
        /// <param name="updateDecoration">
        /// A binary operator that is used to merge the new decoration with existing decorations.
        /// </param>
        public abstract void PushDecoration(
            TextDecoration decoration,
            Func<TextDecoration, TextDecoration, TextDecoration> updateDecoration);

        /// <summary>
        /// Pops an entry from the style stack.
        /// </summary>
        public abstract void PopStyle();
    }
}