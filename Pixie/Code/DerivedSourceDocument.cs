namespace Pixie.Code;

/// <summary>
/// A derived source document whose spans resolve back to original source documents.
/// </summary>
public abstract class DerivedSourceDocument : SourceDocument
{
    /// <inheritdoc/>
    public sealed override LineAndColumnPosition GetPosition(int offset)
    {
        var primary = ResolveSpan(offset, 0).PrimarySpan;
        return primary.Document.GetPosition(primary.Start);
    }
}
