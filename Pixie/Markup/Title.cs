namespace Pixie.Markup
{
    /// <summary>
    /// A centered, emphasized title block.
    /// </summary>
    public sealed class Title : Block
    {
        public Title(string title)
            : this(new Text(title))
        { }

        public Title(Inline title)
        {
            this.Contents = title;
        }

        public Inline Contents { get; private set; }

        public override Block Lower()
        {
            return new AlignBox(
                new Paragraph(
                    new DecorationSpan(
                        Contents,
                        TextDecoration.Bold,
                        DecorationSpan.UnifyDecorations)),
                Alignment.Center);
        }
    }
}
