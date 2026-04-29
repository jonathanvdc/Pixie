using System.Collections.Generic;

namespace Pixie.Markup
{
    /// <summary>
    /// A block node that indents its contents once.
    /// </summary>
    public sealed class IndentBox : BlockContainer
    {
        public IndentBox(Block contents)
            : base(contents)
        { }

        public IndentBox(IReadOnlyList<Block> contents)
            : base(contents)
        { }

        public IndentBox(params Block[] contents)
            : base(contents)
        { }

        public override BlockContainer WithContents(Block newContents)
        {
            return new IndentBox(newContents);
        }
    }
}
