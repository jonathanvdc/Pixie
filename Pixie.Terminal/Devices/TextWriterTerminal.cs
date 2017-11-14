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
        {
            this.Writer = writer;
            this.termWidth = width;
            this.styleManager = styleManager;

            renderableTestEncoding = (Encoding)Writer.Encoding.Clone();
            renderableTestEncoding.EncoderFallback = new RenderableEncoderFallback();
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