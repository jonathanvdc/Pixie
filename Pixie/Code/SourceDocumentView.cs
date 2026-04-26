namespace Pixie.Code
{
    /// <summary>
    /// A derived source document whose spans resolve back to original source documents.
    /// </summary>
    public abstract class SourceDocumentView : SourceDocument
    {
        /// <inheritdoc/>
        public sealed override SourcePosition GetPosition(int offset)
        {
            var primary = ResolveSpan(offset, 0).PrimarySpan;
            return primary.Document.GetPosition(primary.Start);
        }
    }
}
