using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A common base class for list markup nodes.
    /// </summary>
    public abstract class ListNode : Block
    {
        protected ListNode(IReadOnlyList<Block> items)
            : this(items, false)
        { }

        protected ListNode(IReadOnlyList<Block> items, bool separateItems)
        {
            this.Items = items;
            this.SeparateItems = separateItems;
        }

        public IReadOnlyList<Block> Items { get; private set; }

        public bool SeparateItems { get; private set; }
    }

    /// <summary>
    /// A block that defines a list of bulleted items.
    /// </summary>
    public sealed class BulletedList : ListNode
    {
        public BulletedList(IReadOnlyList<Block> items)
            : base(items)
        { }

        public BulletedList(IReadOnlyList<Block> items, bool separateItems)
            : base(items, separateItems)
        { }

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
