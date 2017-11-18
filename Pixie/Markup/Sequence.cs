using System;
using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A markup node that consists of a sequence of other nodes.
    /// </summary>
    public sealed class Sequence : MarkupNode
    {
        /// <summary>
        /// Creates a sequence node from a list of nodes.
        /// </summary>
        /// <param name="contents">A list of nodes to render in sequence.</param>
        public Sequence(params MarkupNode[] contents)
            : this((IReadOnlyList<MarkupNode>)contents)
        { }

        /// <summary>
        /// Creates a sequence node from a list of nodes.
        /// </summary>
        /// <param name="contents">A list of nodes to render in sequence.</param>
        public Sequence(IReadOnlyList<MarkupNode> contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the list of markup nodes that make up this node's contents.
        /// </summary>
        /// <returns>The content markup nodes.</returns>
        public IReadOnlyList<MarkupNode> Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback => null;

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            var newContents = Map(Contents, mapping);
            if (newContents == Contents)
            {
                return this;
            }
            else
            {
                return new Sequence(newContents);
            }
        }

        /// <summary>
        /// Applies a mapping function to a list of markup nodes.
        /// If none of the markup nodes change, then the original
        /// list is returned.
        /// </summary>
        /// <param name="elements">
        /// A read-only view of a list of markup nodes.
        /// </param>
        /// <param name="mapping">
        /// A function that maps markup nodes to markup nodes.
        /// </param>
        /// <returns>
        /// A new list of the mapping function changed at least
        /// one element; otherwise, the original list.
        /// </returns>
        public static IReadOnlyList<MarkupNode> Map(
            IReadOnlyList<MarkupNode> elements,
            Func<MarkupNode, MarkupNode> mapping)
        {
            int count = elements.Count;
            var newElements = new MarkupNode[count];
            bool changed = false;
            for (int i = 0; i < count; i++)
            {
                var elem = elements[i];
                var newElem = mapping(elem);
                newElements[i] = newElem;
                if (elem != newElem)
                {
                    changed = true;
                }
            }

            if (changed)
            {
                return newElements;
            }
            else
            {
                return elements;
            }
        }
    }
}