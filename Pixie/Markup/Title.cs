using System;

namespace Pixie.Markup
{
    /// <summary>
    /// A node that represents a message's title.
    /// </summary>
    public sealed class Title : MarkupNode
    {
        /// <summary>
        /// Creates a title node from the given title.
        /// </summary>
        /// <param name="title">A title string.</param>
        public Title(string title)
        {
            this.Contents = new Text(title);
        }

        /// <summary>
        /// Creates a title node from the given contents.
        /// </summary>
        /// <param name="title">The contents of the title node.</param>
        public Title(MarkupNode title)
        {
            this.Contents = title;
        }

        /// <summary>
        /// Gets the title this node consists of.
        /// </summary>
        /// <returns>The title.</returns>
        public MarkupNode Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback =>
            new Paragraph(
                new AlignBox(
                    Contents,
                    Alignment.Center));
    }
}