using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A definition or reference to a mathematical symbol.
    /// </summary>
    public sealed class MathSymbol : InlineContainer
    {
        /// <summary>
        /// Wraps a markup node into a mathematical symbol.
        /// </summary>
        /// <param name="contents">
        /// The node to render as a mathematical symbol.
        /// </param>
        public MathSymbol(Inline contents)
            : base(contents)
        { }

        /// <inheritdoc/>
        public override Inline Lower()
        {
            return
            new Sequence(
                new DegradableText("⟨", "<"),
                Contents,
                new DegradableText("⟩", ">"));
        }

        /// <inheritdoc/>
        public override InlineContainer WithContents(Inline newContents)
        {
            return new MathSymbol(newContents);
        }
    }
}
