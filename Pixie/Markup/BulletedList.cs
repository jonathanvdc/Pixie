using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A common base class for list markup nodes.
    /// </summary>
    public abstract class ListNode : Block
    {
        /// <summary>
        /// Creates a list node.
        /// </summary>
        /// <param name="items">The list items.</param>
        protected ListNode(IReadOnlyList<Block> items)
            : this(items, false)
        { }

        /// <summary>
        /// Creates a list node.
        /// </summary>
        /// <param name="items">The list items.</param>
        /// <param name="separateItems">
        /// Indicates whether blank lines should be inserted between items.
        /// </param>
        protected ListNode(IReadOnlyList<Block> items, bool separateItems)
        {
            this.Items = items;
            this.SeparateItems = separateItems;
        }

        /// <summary>
        /// Gets the list items.
        /// </summary>
        public IReadOnlyList<Block> Items { get; private set; }

        /// <summary>
        /// Gets a value indicating whether blank lines should separate items.
        /// </summary>
        public bool SeparateItems { get; private set; }
    }

    /// <summary>
    /// A block that defines a list of bulleted items.
    /// </summary>
    public sealed class BulletedList : ListNode
    {
        /// <summary>
        /// Creates a bulleted list.
        /// </summary>
        /// <param name="items">The list items.</param>
        public BulletedList(IReadOnlyList<Block> items)
            : base(items)
        { }

        /// <summary>
        /// Creates a bulleted list.
        /// </summary>
        /// <param name="items">The list items.</param>
        /// <param name="separateItems">
        /// Indicates whether blank lines should be inserted between items.
        /// </param>
        public BulletedList(IReadOnlyList<Block> items, bool separateItems)
            : base(items, separateItems)
        { }

        /// <summary>
        /// Lowers this bulleted list to simpler markup.
        /// </summary>
        /// <returns>The lowered block markup.</returns>
        public override Block Lower()
        {
            var bulletedItems = new List<Block>();
            for (int i = 0; i < Items.Count; i++)
            {
                bulletedItems.Add(
                    new PrefixBox(
                        new DegradableText(" •  ", " *  "),
                        Items[i]));
            }

            return new Stack(bulletedItems);
        }
    }
}
