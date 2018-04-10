using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A definition or reference to a mathematical symbol.
    /// </summary>
    public sealed class MathSymbol : ContainerNode
    {
        /// <summary>
        /// Wraps a markup node into a mathematical symbol.
        /// </summary>
        /// <param name="contents">
        /// The node to render as a mathematical symbol.
        /// </param>
        public MathSymbol(MarkupNode contents)
            : base(contents)
        { }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Sequence(
                new DegradableText("⟨", "<"),
                Contents,
                new DegradableText("⟩", ">"));

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new MathSymbol(newContents);
        }
    }
}
