using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pixie.Terminal.Devices
{
    /// <summary>
    /// A terminal that writes to a text writer.
    /// </summary>
    public class TextWriterTerminal : OutputTerminalBase
    {
        /// <summary>
        /// Creates a text writer terminal from a text writer
        /// and a terminal width.
        /// </summary>
        /// <param name="writer">A text writer to send output to.</param>
        /// <param name="width">The width of the output document.</param>
        public TextWriterTerminal(
            TextWriter writer,
            int width)
            : this(writer, width, NoStyleManager.Instance)
        { }

        /// <summary>
        /// Creates a text writer terminal from a text writer,
        /// a terminal width and a style manager.
        /// </summary>
        /// <param name="writer">A text writer to send output to.</param>
        /// <param name="width">The width of the output document.</param>
        /// <param name="styleManager">A style manager for the output document.</param>
        public TextWriterTerminal(
            TextWriter writer,
            int width,
            StyleManager styleManager)
            : this(writer, width, styleManager, writer.Encoding)
        { }

        /// <summary>
        /// Creates a text writer terminal from a text writer,
        /// a terminal width and a style manager.
        /// </summary>
        /// <param name="writer">A text writer to send output to.</param>
        /// <param name="width">The width of the output document.</param>
        /// <param name="styleManager">A style manager for the output document.</param>
        /// <param name="renderableEncoding">
        /// The encoding that is used for determining which strings are renderable and which are not.
        /// </param>
        public TextWriterTerminal(
            TextWriter writer,
            int width,
            StyleManager styleManager,
            Encoding renderableEncoding)
        {
            this.Writer = writer;
            this.termWidth = width;
            this.styleManager = styleManager;

            this.renderableTestEncoding = (Encoding)renderableEncoding.Clone();
            this.renderableTestEncoding.EncoderFallback = new RenderableEncoderFallback();
        }

        /// <summary>
        /// Gets the text writer to which output is sent by this terminal.
        /// </summary>
        /// <returns>A text writer.</returns>
        public TextWriter Writer { get; private set; }

        private int termWidth;

        private Encoding renderableTestEncoding;

        /// <inheritdoc/>
        public override int Width => termWidth;

        private StyleManager styleManager;

        /// <inheritdoc/>
        public override StyleManager Style => styleManager;

        /// <inheritdoc/>
        public override void Write(string text)
        {
            EndSeparator();
            Writer.Write(text);
        }

        /// <inheritdoc/>
        protected override void WriteLineImpl()
        {
            Writer.WriteLine();
        }

        /// <inheritdoc/>
        public override void Write(char character)
        {
            EndSeparator();
            Writer.Write(character);
        }

        /// <inheritdoc/>
        public override bool CanRender(string text)
        {
            var fallback = (RenderableEncoderFallback)renderableTestEncoding.EncoderFallback;
            fallback.FoundUnencodable = false;
            renderableTestEncoding.GetBytes(text);
            return !fallback.FoundUnencodable;
        }

        /// <summary>
        /// Creates a text writer terminal from the given console stream and
        /// a flag that tells if said stream's output is being redirected.
        /// <param name="writer">
        /// A writer to write output to.
        /// </param>
        /// </summary>
        /// <param name="isRedirected">
        /// Tells if the writer's output is being redirected.
        /// </param>
        /// <param name="styleManagerOrNull">
        /// The style manager to use. Pass <c>null</c> to automatically
        /// pick a style manager based on the terminal.
        /// </param>
        /// <param name="terminalWidthOrNull">
        /// The size of the terminal to use. Pass <c>null</c> to automatically
        /// pick the width based on the terminal.
        /// </param>
        /// <returns>A text writer terminal.</returns>
        private static TextWriterTerminal FromConsoleStream(
            TextWriter writer,
            bool isRedirected,
            StyleManager styleManagerOrNull,
            Nullable<int> terminalWidthOrNull)
        {
            if (isRedirected)
            {
                // Don't style redirected text.
                return new TextWriterTerminal(
                    writer,
                    terminalWidthOrNull.HasValue
                        ? terminalWidthOrNull.Value
                        : DefaultTerminalWidth,
                    styleManagerOrNull ?? NoStyleManager.Instance,
                    GetSafeEncoding(writer));
            }
            else if (IsAnsiTerminalIdentifier(
                EnvironmentTerminalIdentifier.ToLowerInvariant()))
            {
                // Only use ANSI control sequences if the terminal
                // appears receptive.
                return new TextWriterTerminal(
                    writer,
                    terminalWidthOrNull.HasValue
                        ? terminalWidthOrNull.Value
                        : GetTerminalWidth(),
                    styleManagerOrNull ?? new AnsiStyleManager(writer));
            }
            else
            {
                // As a fallback, use 'System.Console' for styles.
                // Also, don't trust the writer's encoding on Windows;
                // restrict ourselves to ASCII.
                return new TextWriterTerminal(
                    writer,
                    terminalWidthOrNull.HasValue
                        ? terminalWidthOrNull.Value
                        : GetTerminalWidth(),
                    styleManagerOrNull ?? new ConsoleStyleManager(),
                    GetSafeEncoding(writer));
            }
        }

        /// <summary>
        /// Creates a text writer terminal from the console output stream,
        /// a style manager and the width of the output buffer.
        /// </summary>
        /// <param name="styleManager">
        /// A style manager to style the output with.
        /// </param>
        /// <param name="width">
        /// The width of the output buffer.
        /// </param>
        /// <returns>A text writer terminal.</returns>
        public static TextWriterTerminal FromOutputStream(
            StyleManager styleManager,
            int width)
        {
            return FromConsoleStream(
                Console.Out,
                Console.IsOutputRedirected,
                styleManager,
                new Nullable<int>(width));
        }

        /// <summary>
        /// Creates a text writer terminal from the console output stream
        /// and a style manager.
        /// </summary>
        /// <param name="styleManager">
        /// A style manager to style the output with.
        /// </param>
        /// <returns>A text writer terminal.</returns>
        public static TextWriterTerminal FromOutputStream(
            StyleManager styleManager)
        {
            return FromConsoleStream(
                Console.Out,
                Console.IsOutputRedirected,
                styleManager,
                default(Nullable<int>));
        }

        /// <summary>
        /// Creates a text writer terminal from the console output stream.
        /// </summary>
        /// <returns>A text writer terminal.</returns>
        public static TextWriterTerminal FromOutputStream()
        {
            return FromConsoleStream(
                Console.Out,
                Console.IsOutputRedirected,
                null,
                default(Nullable<int>));
        }

        /// <summary>
        /// Creates a text writer terminal from the console error stream,
        /// a style manager and the width of the output buffer.
        /// </summary>
        /// <param name="styleManager">
        /// A style manager to style the output with.
        /// </param>
        /// <param name="width">
        /// The width of the output buffer.
        /// </param>
        /// <returns>A text writer terminal.</returns>
        public static TextWriterTerminal FromErrorStream(
            StyleManager styleManager,
            int width)
        {
            return FromConsoleStream(
                Console.Error,
                Console.IsErrorRedirected,
                styleManager,
                new Nullable<int>(width));
        }

        /// <summary>
        /// Creates a text writer terminal from the console error stream
        /// and a style manager.
        /// </summary>
        /// <param name="styleManager">
        /// A style manager to style the output with.
        /// </param>
        /// <returns>A text writer terminal.</returns>
        public static TextWriterTerminal FromErrorStream(
            StyleManager styleManager)
        {
            return FromConsoleStream(
                Console.Error,
                Console.IsErrorRedirected,
                styleManager,
                default(Nullable<int>));
        }

        /// <summary>
        /// Creates a text writer terminal from the console error stream.
        /// </summary>
        /// <returns>A text writer terminal.</returns>
        public static TextWriterTerminal FromErrorStream()
        {
            return FromConsoleStream(
                Console.Error,
                Console.IsErrorRedirected,
                null,
                default(Nullable<int>));
        }

        /// <summary>
        /// Gets the environment's terminal identifier, if any.
        /// </summary>
        /// <returns>A terminal identifier.</returns>
        public static string EnvironmentTerminalIdentifier =>
            Environment.GetEnvironmentVariable("TERM");

        private static bool IsAnsiTerminalIdentifier(string identifier)
        {
            return identifier.StartsWith("vt")
                || identifier.StartsWith("xterm")
                || identifier == "linux";
        }

        private static Encoding GetSafeEncoding(TextWriter writer)
        {
            return WindowsIsOS() ? Encoding.ASCII : writer.Encoding;
        }

        private static bool WindowsIsOS()
        {
            var platformId = Environment.OSVersion.Platform;
            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    return true;
                default:
                    return false;
            }
        }

        private const int DefaultTerminalWidth = 80;

        private static int GetTerminalWidth()
        {
            int result = 0;
            try
            {
                result = Console.BufferWidth;
            }
            catch (Exception)
            {
                // Console.BufferWidth can throw when using the
                // .NET framework on Cygwin. We'll just ignore
                // the error and pick the default size.
            }

            return result <= 0
                ? DefaultTerminalWidth
                : result;
        }
    }

    internal sealed class RenderableEncoderFallback : EncoderFallback
    {
        public override int MaxCharCount { get { return 0; } }
        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new RenderableEncoderFallbackBuffer(this);
        }

        /// <summary>
        /// Gets or sets a Boolean that tells if any unencodable
        /// characters were found.
        /// </summary>
        /// <returns>
        /// <c>true</c> if any unencodable characters were found; otherwise, <c>false</c>.
        /// </returns>
        public bool FoundUnencodable { get; set; }
    }

    internal sealed class RenderableEncoderFallbackBuffer : EncoderFallbackBuffer
    {
        public RenderableEncoderFallbackBuffer(RenderableEncoderFallback owner)
        {
            this.owner = owner;
        }

        private RenderableEncoderFallback owner;

        public override int Remaining { get { return 0; } }

        public override bool Fallback(char unknownChar, int index)
        {
            owner.FoundUnencodable = true;

            return true;
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            return false;
        }

        public override char GetNextChar()
        {
            return default(char);
        }

        public override bool MovePrevious()
        {
            return false;
        }

        public override void Reset()
        {
        }
    }
}