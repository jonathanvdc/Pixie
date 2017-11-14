using System;
using System.Collections.Generic;
using System.IO;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that uses the 'System.Console' class directly.
    /// </summary>
    public sealed class ConsoleTerminal : TextWriterTerminal
    {
        public ConsoleTerminal()
            : base(
                Console.Error,
                Console.WindowWidth,
                new ConsoleStyleManager(
                    ConsoleStyle.ToPixieColor(Console.ForegroundColor),
                    ConsoleStyle.ToPixieColor(Console.BackgroundColor)))
        { }
    }

    public sealed class ConsoleStyleManager : StyleManager
    {
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

        public override void PushForegroundColor(Color color)
        {
            var curStyle = CurrentStyle;
            PushStyle(
                new ConsoleStyle(
                    color.Over(curStyle.ForegroundColor),
                    curStyle.BackgroundColor));
        }

        public override void PushBackgroundColor(Color color)
        {
            var curStyle = CurrentStyle;
            PushStyle(
                new ConsoleStyle(
                    curStyle.ForegroundColor,
                    color.Over(curStyle.BackgroundColor)));
        }

        private void PushStyle(ConsoleStyle style)
        {
            style.Apply(CurrentStyle);
            styleStack.Push(style);
        }

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
                { ConsoleColor.Blue, Colors.Blue },
                { ConsoleColor.Cyan, Colors.Cyan },
                { ConsoleColor.Gray, Colors.Gray },
                { ConsoleColor.Green, Colors.Green },
                { ConsoleColor.Magenta, Colors.Magenta },
                { ConsoleColor.Red, Colors.Red },
                { ConsoleColor.White, Colors.White },
                { ConsoleColor.Yellow, Colors.Yellow },
                { ConsoleColor.DarkBlue, MakeDark(Colors.Blue) },
                { ConsoleColor.DarkCyan, MakeDark(Colors.Cyan) },
                { ConsoleColor.DarkGray, MakeDark(Colors.Gray) },
                { ConsoleColor.DarkGreen, MakeDark(Colors.Green) },
                { ConsoleColor.DarkMagenta, MakeDark(Colors.Magenta) },
                { ConsoleColor.DarkRed, MakeDark(Colors.Red) },
                { ConsoleColor.DarkYellow, MakeDark(Colors.Yellow) }
            };
        }

        private static Dictionary<ConsoleColor, Color> colorMap;

        private static Color MakeDark(Color color)
        {
            double factor = 0.5;
            return new Color(
                factor * color.Red,
                factor * color.Green,
                factor * color.Blue,
                color.Alpha);
        }

        public static Color ToPixieColor(ConsoleColor color)
        {
            return colorMap[color];
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