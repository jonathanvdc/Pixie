using System;
using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A style manager that applies styles by setting the color
    /// properties of the 'System.Console' class.
    /// </summary>
    public sealed class ConsoleStyleManager : StyleManager
    {
        /// <summary>
        /// Creates a console style manager.
        /// </summary>
        public ConsoleStyleManager()
            : this(
                ConsoleStyle.ToPixieColor(Console.ForegroundColor, Colors.White),
                ConsoleStyle.ToPixieColor(Console.BackgroundColor, Colors.Black))
        { }

        /// <summary>
        /// Creates a console style manager from a default foreground
        /// and background color.
        /// </summary>
        /// <param name="defaultForegroundColor">The default foreground color.</param>
        /// <param name="defaultBackgroundColor">The default background color.</param>
        public ConsoleStyleManager(
            Color defaultForegroundColor,
            Color defaultBackgroundColor)
        {
            this.styleStack = new Stack<ConsoleStyle>();
            this.styleStack.Push(
                new ConsoleStyle(
                    defaultForegroundColor,
                    defaultBackgroundColor,
                    true));
        }

        private Stack<ConsoleStyle> styleStack;

        private ConsoleStyle CurrentStyle => styleStack.Peek();

        /// <inheritdoc/>
        public override void PushForegroundColor(Color color)
        {
            var curStyle = CurrentStyle;
            PushStyle(
                new ConsoleStyle(
                    color.Over(curStyle.ForegroundColor),
                    curStyle.BackgroundColor));
        }

        /// <inheritdoc/>
        public override void PushBackgroundColor(Color color)
        {
            var curStyle = CurrentStyle;
            PushStyle(
                new ConsoleStyle(
                    curStyle.ForegroundColor,
                    color.Over(curStyle.BackgroundColor)));
        }

        /// <inheritdoc/>
        public override void PushDecoration(
            TextDecoration decoration,
            Func<TextDecoration, TextDecoration, TextDecoration> updateDecoration)
        {
            // 'System.Console' doesn't support text decorations. Just push
            // the current style.
            PushStyle(CurrentStyle);
        }

        private void PushStyle(ConsoleStyle style)
        {
            style.Apply(CurrentStyle);
            styleStack.Push(style);
        }

        /// <inheritdoc/>
        public override void PopStyle()
        {
            var popped = styleStack.Pop();
            CurrentStyle.Apply(popped);
        }
    }

    internal sealed class ConsoleStyle
    {
        public ConsoleStyle(
            Color foregroundColor,
            Color backgroundColor)
            : this(foregroundColor, backgroundColor, false)
        { }

        public ConsoleStyle(
            Color foregroundColor,
            Color backgroundColor,
            bool isRootStyle)
        {
            this.ForegroundColor = foregroundColor;
            this.BackgroundColor = backgroundColor;
            this.IsRootStyle = isRootStyle;
        }

        public Color ForegroundColor { get; private set; }

        public Color BackgroundColor { get; private set; }

        public bool IsRootStyle { get; private set; }

        /// <summary>
        /// Applies this style, given a previous style.
        /// </summary>
        public void Apply(ConsoleStyle style)
        {
            var newFg = ToConsoleColor(ForegroundColor);
            var newBg = ToConsoleColor(BackgroundColor);

            if (IsRootStyle)
            {
                Console.ResetColor();
                return;
            }

            if (Console.ForegroundColor != newFg)
            {
                Console.ForegroundColor = newFg;
            }

            if (Console.BackgroundColor != newBg)
            {
                Console.BackgroundColor = newBg;
            }
        }

        static ConsoleStyle()
        {
            colorMap = new Dictionary<ConsoleColor, Color>()
            {
                { ConsoleColor.Black, Colors.Black },
                { ConsoleColor.DarkBlue, Colors.Blue },
                { ConsoleColor.DarkCyan, Colors.Cyan },
                { ConsoleColor.DarkGreen, Colors.Green },
                { ConsoleColor.DarkMagenta, Colors.Magenta },
                { ConsoleColor.DarkRed, Colors.Red },
                { ConsoleColor.DarkYellow, Colors.Yellow },
                { ConsoleColor.Gray, Colors.White },
                { ConsoleColor.DarkGray, Colors.Gray },
                { ConsoleColor.Blue, new Color(0.0, 0.0, 1.0) },
                { ConsoleColor.Cyan, new Color(0.0, 1.0, 1.0) },
                { ConsoleColor.Green, new Color(0.0, 1.0, 0.0) },
                { ConsoleColor.Magenta, new Color(1.0, 0.0, 1.0) },
                { ConsoleColor.Red, new Color(1.0, 0.0, 0.0) },
                { ConsoleColor.White, new Color(1.0) },
                { ConsoleColor.Yellow, new Color(1.0, 1.0, 0.0) }
            };
        }

        private static Dictionary<ConsoleColor, Color> colorMap;

        public static Color ToPixieColor(ConsoleColor color, Color fallbackResult)
        {
            Color result;
            if (colorMap.TryGetValue(color, out result))
            {
                return result;
            }
            else
            {
                return fallbackResult;
            }
        }

        public static ConsoleColor ToConsoleColor(Color color)
        {
            var nearestColor = ConsoleColor.Gray;
            var nearestColorDistSqr = 3.0;
            foreach (var pair in colorMap)
            {
                var otherColor = pair.Value;
                var distR = otherColor.Red - color.Red;
                var distG = otherColor.Green - color.Green;
                var distB = otherColor.Blue - color.Blue;
                var distSqr = distR * distR + distG * distG + distB * distB;
                if (distSqr < nearestColorDistSqr)
                {
                    nearestColorDistSqr = distSqr;
                    nearestColor = pair.Key;
                }
            }
            return nearestColor;
        }
    }
}
