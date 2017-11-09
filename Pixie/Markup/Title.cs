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
        /// <param name="Title">A title.</param>
        public Title(string title)
        {
            this.Contents = title;
        }

        /// <summary>
        /// Gets the title this node consists of.
        /// </summary>
        /// <returns>The title.</returns>
        public string Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback => new Sequence(new Text("# " + Contents), NewLine.Instance);
    }
}