namespace Pixie.Markup
{
    /// <summary>
    /// An enumeration of possible alignments.
    /// </summary>
    public enum Alignment
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// A block that aligns its contents inside the available width.
    /// </summary>
    public sealed class AlignBox : BlockContainer
    {
        public AlignBox(Block contents)
            : this(contents, Alignment.Left)
        { }

        public AlignBox(Block contents, Alignment alignment)
            : base(contents)
        {
            this.Alignment = alignment;
        }

        public Alignment Alignment { get; private set; }

        public override BlockContainer WithContents(Block newContents)
        {
            return new AlignBox(newContents, Alignment);
        }
    }
}
