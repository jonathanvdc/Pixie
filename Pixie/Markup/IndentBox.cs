using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A box node that indents its contents once.
    /// </summary>
    public sealed class IndentBox : ContainerNode
    {
        /// <summary>
        /// Creates a box node that indents its contents once.
        /// </summary>
        /// <param name="contents">The contents to indent.</param>
        public IndentBox(MarkupNode contents)
            : base(contents)
        { }

        /// <summary>
        /// Creates a box node that indents its contents once.
        /// </summary>
        /// <param name="contents">The contents to indent.</param>
        public IndentBox(IReadOnlyList<MarkupNode> contents)
            : base(contents)
        { }

        /// <summary>
        /// Creates a box node that indents its contents once.
        /// </summary>
        /// <param name="contents">The contents to indent.</param>
        public IndentBox(params MarkupNode[] contents)
            : base(contents)
        { }

        /// <inheritdoc/>
        public override MarkupNode Fallback => WrapBox.IndentAndWordWrap(Contents);

        /// <inheritdoc/>
        public override ContainerNode WithContents(MarkupNode newContents)
        {
            return new IndentBox(newContents);
        }
    }
}
