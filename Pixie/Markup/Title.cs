using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A node that represents a message's title.
    /// </summary>
    public sealed class Title : ContainerNode
    {
        /// <summary>
        /// Creates a title node from the given title.
        /// </summary>
        /// <param name="title">A title string.</param>
        public Title(string title)
            : this(new Text(title))
        { }

        /// <summary>
        /// Creates a title node from the given contents.
        /// </summary>
        /// <param name="title">The contents of the title node.</param>
        public Title(MarkupNode title)
            : base(title)
        { }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Paragraph(
                new AlignBox(
                    new DecorationSpan(
                        Contents,
                        TextDecoration.Bold,
                        DecorationSpan.UnifyDecorations),
                    Alignment.Center));
    }
}