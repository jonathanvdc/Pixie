using System;
using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// Base type for phrase-level markup that can flow within a line or
    /// paragraph.
    /// </summary>
    public abstract class Inline : MarkupElement
    {
        /// <summary>
        /// Attempts to express this inline node in simpler semantic markup.
        /// </summary>
        public virtual Inline Lower()
        {
            return null;
        }

        /// <summary>
        /// Creates inline text from a string.
        /// </summary>
        public static implicit operator Inline(string text)
        {
            return new Text(text);
        }
    }

    /// <summary>
    /// Base type for inline nodes that contain a single inline child.
    /// </summary>
    public abstract class InlineContainer : Inline
    {
        protected InlineContainer(Inline contents)
        {
            this.Contents = contents;
        }

        protected InlineContainer(IReadOnlyList<Inline> contents)
            : this(new Sequence(contents))
        { }

        /// <summary>
        /// Gets the contained inline markup.
        /// </summary>
        public Inline Contents { get; private set; }

        /// <summary>
        /// Creates a copy of this node with new contents.
        /// </summary>
        public abstract InlineContainer WithContents(Inline newContents);
    }
}
