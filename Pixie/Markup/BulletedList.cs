using System;
using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A common base class for list markup nodes.
    /// </summary>
    public abstract class ListNode : MarkupNode
    {
        /// <summary>
        /// Creates a list from a sequence of list items.
        /// </summary>
        /// <param name="items">The items in the list.</param>
        public ListNode(IReadOnlyList<MarkupNode> items)
            : this(items, false)
        { }

        /// <summary>
        /// Creates a list from a sequence of list items.
        /// </summary>
        /// <param name="items">The items in the list.</param>
        /// <param name="separateItems">
        /// Tells if the list's items should be separated by
        /// vertical whitespace.
        /// </param>
        public ListNode(IReadOnlyList<MarkupNode> items, bool separateItems)
        {
            this.Items = items;
            this.SeparateItems = separateItems;
        }

        /// <summary>
        /// Gets the items in this list.
        /// </summary>
        /// <returns>The list's items.</returns>
        public IReadOnlyList<MarkupNode> Items { get; private set; }

        /// <summary>
        /// Tells if the list's items should be separated by
        /// vertical whitespace.
        /// </summary>
        /// <returns>
        /// <c>true</c> if items should be separated by vertical whitespace; otherwise, <c>false</c>.
        /// </returns>
        public bool SeparateItems { get; private set; }
    }

    public sealed class BulletedList : ListNode
    {
        /// <summary>
        /// Creates a bulleted list from a sequence of list items.
        /// </summary>
        /// <param name="items">The items in the list.</param>
        public BulletedList(IReadOnlyList<MarkupNode> items)
            : base(items)
        { }

        /// <summary>
        /// Creates a bulleted list from a sequence of list items.
        /// </summary>
        /// <param name="items">The items in the list.</param>
        /// <param name="separateItems">
        /// Tells if the list's items should be separated by
        /// vertical whitespace.
        /// </param>
        public BulletedList(IReadOnlyList<MarkupNode> items, bool separateItems)
            : base(items, separateItems)
        { }

        /// <inheritdoc/>
        public override MarkupNode Fallback
        {
            get
            {
                var bulletedItems = new List<MarkupNode>();
                for (int i = 0; i < Items.Count; i++)
                {
                    MarkupNode bulleted = new PrefixBox(
                        new DegradableText(" •  ", " *  "),
                        Items[i]);

                    if (SeparateItems)
                    {
                        bulleted = new Paragraph(bulleted);
                    }

                    bulletedItems.Add(bulleted);
                }

                return new Sequence(bulletedItems);
            }
        }

        /// <inheritdoc/>
        public override MarkupNode Map(Func<MarkupNode, MarkupNode> mapping)
        {
            var newItems = Sequence.Map(Items, mapping);
            if (newItems == Items)
            {
                return this;
            }
            else
            {
                return new BulletedList(newItems, SeparateItems);
            }
        }
    }
}