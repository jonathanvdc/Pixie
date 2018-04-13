using System;
using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A node that visualizes a diff of a pair of strings.
    /// </summary>
    public sealed class TextDiff : MarkupNode
    {
        /// <summary>
        /// Creates a text diff node.
        /// </summary>
        /// <param name="diff">
        /// The diff to render.
        /// </param>
        /// <param name="operationColors">
        /// A dictionary that maps diff operations to the color in which
        /// diff elements that use those operations are painted. Diff operations
        /// that do not appear as keys in the dictionary are not rendered, except
        /// for <see cref="Pixie.DiffOperation.Unchanged"/>, which is rendered
        /// without color if it does not show up as a key.
        /// </param>
        public TextDiff(
            Diff<char> diff,
            IReadOnlyDictionary<DiffOperation, Color> operationColors)
        {
            this.Diff = diff;
            this.OperationColors = operationColors;
        }

        /// <summary>
        /// Gets the diff to render.
        /// </summary>
        /// <returns>The diff to render.</returns>
        public Diff<char> Diff { get; private set; }

        /// <summary>
        /// Gets a dictionary that maps diff operations to the color in which
        /// diff elements that use those operations are painted. Diff operations
        /// that do not appear as keys in the dictionary are not rendered, except
        /// for <see cref="Pixie.DiffOperation.Unchanged"/>, which is rendered
        /// without color if it does not show up as a key.
        /// </summary>
        /// <returns>A mapping of diff operations to colors.</returns>
        public IReadOnlyDictionary<DiffOperation, Color> OperationColors { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Fallback
        {
            get
            {
                var nodes = new List<MarkupNode>();
                int elemCount = Diff.Elements.Count;
                for (int i = 0; i < elemCount; i++)
                {
                    var elem = Diff.Elements[i];
                    Color color;
                    if (OperationColors.TryGetValue(elem.Operation, out color))
                    {
                        nodes.Add(
                            new ColorSpan(
                                string.Concat<char>(elem.Values),
                                color));
                    }
                    else if (elem.Operation == DiffOperation.Unchanged)
                    {
                        nodes.Add(string.Concat<char>(elem.Values));
                    }
                }
                return new Sequence(nodes);
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            return this;
        }

        /// <summary>
        /// Creates a text diff that renders the deletions in a diff.
        /// </summary>
        /// <param name="diff">A diff to render.</param>
        /// <returns>
        /// A text diff node that renders deletions and unchanged diff elements only.
        /// </returns>
        public static TextDiff RenderDeletions(Diff<char> diff)
        {
            return new TextDiff(
                diff,
                new Dictionary<DiffOperation, Color>()
                {
                    { DiffOperation.Deletion, Colors.Red }
                });
        }

        /// <summary>
        /// Creates a text diff that renders the insertions in a diff.
        /// </summary>
        /// <param name="diff">A diff to render.</param>
        /// <returns>
        /// A text diff node that renders insertions and unchanged diff elements only.
        /// </returns>
        public static TextDiff RenderInsertions(Diff<char> diff)
        {
            return new TextDiff(
                diff,
                new Dictionary<DiffOperation, Color>()
                {
                    { DiffOperation.Insertion, Colors.Green }
                });
        }

        /// <summary>
        /// Creates a text diff that renders the both the deletions
        /// and insertions in a diff.
        /// </summary>
        /// <param name="diff">A diff to render.</param>
        /// <returns>
        /// A text diff node that renders all diff elements.
        /// </returns>
        public static TextDiff RenderAll(Diff<char> diff)
        {
            return new TextDiff(
                diff,
                new Dictionary<DiffOperation, Color>()
                {
                    { DiffOperation.Deletion, Colors.Red },
                    { DiffOperation.Insertion, Colors.Green }
                });
        }
    }
}