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
                    defaultBackgroundColor));
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
        public ConsoleStyle(Color foregroundColor, Color backgroundColor)
        {
            this.ForegroundColor = foregroundColor;
            this.BackgroundColor = backgroundColor;
        }

        public Color ForegroundColor { get; private set; }

        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Applies this style, given a previous style.
        /// </summary>
        public void Apply(ConsoleStyle style)
        {
            // TODO: implement this
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

            invColorMap = new Dictionary<Color, ConsoleColor>();
            foreach (var pair in colorMap)
            {
                invColorMap[pair.Value] = pair.Key;
            }
        }

        private static Dictionary<ConsoleColor, Color> colorMap;
        private static Dictionary<Color, ConsoleColor> invColorMap;

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
    }
}