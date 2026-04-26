namespace Pixie.Code;

/// <summary>
/// Represents a span in user-authored original source text.
/// </summary>
/// <remarks>
/// Unlike <see cref="SourceSpan"/>, this type is not relative to an arbitrary parsed
/// source view. It always points at an <see cref="OriginalSourceDocument"/>.
/// </remarks>
/// <remarks>
/// Creates an original source span from a document, start offset and length.
/// </remarks>
/// <param name="document">The original source document.</param>
/// <param name="start">The zero-based start offset.</param>
/// <param name="length">The span length.</param>
public readonly struct OriginalSourceSpan(
    OriginalSourceDocument document,
    int start,
    int length)
{

    /// <summary>
    /// Gets the original source document that owns this span.
    /// </summary>
    /// <returns>The original source document.</returns>
    public OriginalSourceDocument Document { get; } = document;

    /// <summary>
    /// Gets the zero-based start offset.
    /// </summary>
    /// <returns>The zero-based start offset.</returns>
    public int Start { get; } = start;

    /// <summary>
    /// Gets the span length.
    /// </summary>
    /// <returns>The span length.</returns>
    public int Length { get; } = length;

    /// <summary>
    /// Gets the exclusive end offset.
    /// </summary>
    /// <returns>The exclusive end offset.</returns>
    public int End => Start + Length;
}
