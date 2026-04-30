using System;
using System.Text;

namespace Pixie
{
    /// <summary>
    /// Describes an RGBA color.
    /// </summary>
    public readonly struct Color
    {
        /// <summary>
        /// Creates a new color instance from the given channels.
        /// </summary>
        public Color(double red, double green, double blue, double alpha)
        {
            this = default(Color);
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }

        /// <summary>
        /// Creates a new color instance from the given RGB channels.
        /// Alpha is set to one.
        /// </summary>
        public Color(double red, double green, double blue)
            : this(red, green, blue, 1.0)
        { }

        /// <summary>
        /// Creates a new color instance from the given grayscale and alpha values.
        /// </summary>
        public Color(double grayscale, double alpha)
            : this(grayscale, grayscale, grayscale, alpha)
        { }

        /// <summary>
        /// Creates a new color instance from the given grayscale value.
        /// Alpha is set to one.
        /// </summary>
        public Color(double grayscale)
            : this(grayscale, 1.0)
        { }

        /// <summary>
        /// Gets the color's alpha channel.
        /// </summary>
        public double Alpha { get; }

        /// <summary>
        /// Gets the color's red channel.
        /// </summary>
        public double Red { get; }

        /// <summary>
        /// Gets the color's green channel.
        /// </summary>
        public double Green { get; }

        /// <summary>
        /// Gets the color's blue channel.
        /// </summary>
        public double Blue { get; }

        /// <summary>
        /// Gets the color's grayscale intensity. The alpha channel
        /// is not considered in this computation.
        /// </summary>
        public double Grayscale => (Red + Green + Blue) / 3.0;

        /// <summary>
        /// Applies the "over" alpha blending operator to this color and the given
        /// other color.
        /// </summary>
        public Color Over(Color Other)
        {
            double otherAlpha = Other.Alpha * (1.0 - Alpha);
            double ao = Alpha + otherAlpha;
            double ro = Red * Alpha + Other.Red * otherAlpha;
            double go = Green * Alpha + Other.Green * otherAlpha;
            double bo = Blue * Alpha + Other.Blue * otherAlpha;
            return new Color(ro, go, bo, ao);
        }

        private void AppendChannel(StringBuilder sb, string Name, double Value)
        {
            sb.Append(Name);
            sb.Append(":");
            sb.Append(Value);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            AppendChannel(sb, "a", Alpha);
            sb.Append(";");
            AppendChannel(sb, "r", Red);
            sb.Append(";");
            AppendChannel(sb, "g", Green);
            sb.Append(";");
            AppendChannel(sb, "b", Blue);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Defines common colors using the traditional terminal ANSI palette.
    /// </summary>
    public static class Colors
    {
        private const double NormalIntensity = 0.8;
        private const double BrightBlackIntensity = 1.0 / 3.0;
        private const double NormalWhiteIntensity = 0.9601849913597107;

        /// <summary>
        /// Gets the transparent color, which does nothing when merged on
        /// top of another color.
        /// </summary>
        /// <returns>The transparent color.</returns>
        public static Color Transparent => new Color(0.0, 0.0, 0.0, 0.0);

        /// <summary>
        /// Gets the color black.
        /// </summary>
        /// <returns>The color black.</returns>
        public static Color Black => new Color(0.0);

        /// <summary>
        /// Gets the color white.
        /// </summary>
        /// <returns>The color white.</returns>
        public static Color White => new Color(NormalWhiteIntensity);

        /// <summary>
        /// Gets the color gray.
        /// </summary>
        /// <returns>The color gray.</returns>
        public static Color Gray => new Color(BrightBlackIntensity);

        /// <summary>
        /// Gets the color red.
        /// </summary>
        /// <returns>The color red.</returns>
        public static Color Red => new Color(NormalIntensity, 0.0, 0.0);

        /// <summary>
        /// Gets the color green.
        /// </summary>
        /// <returns>The color green.</returns>
        public static Color Green => new Color(0.0, NormalIntensity, 0.0);

        /// <summary>
        /// Gets the color blue.
        /// </summary>
        /// <returns>The color blue.</returns>
        public static Color Blue => new Color(0.0, 0.0, NormalIntensity);

        /// <summary>
        /// Gets the color yellow.
        /// </summary>
        /// <returns>The color yellow.</returns>
        public static Color Yellow => new Color(NormalIntensity, NormalIntensity, 0.0);

        /// <summary>
        /// Gets the color magenta.
        /// </summary>
        /// <returns>The color magenta.</returns>
        public static Color Magenta => new Color(NormalIntensity, 0.0, NormalIntensity);

        /// <summary>
        /// Gets the color cyan.
        /// </summary>
        /// <returns>The color cyan.</returns>
        public static Color Cyan => new Color(0.0, NormalIntensity, NormalIntensity);
    }
}
