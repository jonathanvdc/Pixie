using System;
using Pixie.Markup;

namespace Pixie
{
    /// <summary>
    /// A base class for markup nodes: composable elements that can
    /// be rendered.
    /// </summary>
    public abstract class MarkupNode
    {
        /// <summary>
        /// Gets a fallback version of this node for when the renderer
        /// doesn't know how to render this node's type. Null is
        /// returned if no reasonable fallback can be provided.
        /// </summary>
        /// <returns>The node's fallback version, or <c>null</c>.</returns>
        public abstract MarkupNode Fallback { get; }

        /// <summary>
        /// Applies a mapping to this markup node's children and returns
        /// a new instance of this node's type that contains the modified
        /// children.
        /// </summary>
        /// <param name="mapping">A mapping function.</param>
        /// <returns>A new markup node.</returns>
        public abstract MarkupNode Map(Func<MarkupNode, MarkupNode> mapping);

        /// <summary>
        /// Creates a text markup node from a string of characters.
        /// </summary>
        /// <param name="text">
        /// The text to wrap in a text markup node.
        /// </param>
        public static implicit operator MarkupNode(string text)
        {
            return new Text(text);
        }
    }
}
