using System;
using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A sequence of inline markup nodes.
    /// </summary>
    public sealed class Sequence : Inline
    {
        public Sequence(params Inline[] contents)
            : this((IReadOnlyList<Inline>)contents)
        { }

        public Sequence(IReadOnlyList<Inline> contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the sequence's inline children.
        /// </summary>
        public IReadOnlyList<Inline> Contents { get; private set; }

        /// <summary>
        /// Maps a sequence without reallocating when no element changes.
        /// </summary>
        public static IReadOnlyList<T> Map<T>(
            IReadOnlyList<T> elements,
            Func<T, T> mapping)
            where T : class
        {
            int count = elements.Count;
            var newElements = new T[count];
            bool isSame = true;
            for (int i = 0; i < count; i++)
            {
                newElements[i] = mapping(elements[i]);
                isSame &= object.ReferenceEquals(elements[i], newElements[i]);
            }
            return isSame ? elements : newElements;
        }
    }
}
