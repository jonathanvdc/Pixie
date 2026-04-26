using System.IO;
using System.Text;

namespace Pixie.Code;

/// <summary>
/// Represents a source text buffer that can be consumed by a lexer or parser.
/// </summary>
/// <remarks>
/// <para>
/// A source document may be an original user-authored file or a derived view such as
/// preprocessed text. <see cref="SourceSpan"/> values store offsets relative to this
/// document's text, while document implementations decide how those offsets
/// resolve back to original source coordinates for diagnostics.
/// </para>
/// <para>
/// Public position lookup returns diagnostic coordinates. For mapped documents, those
/// coordinates may come from an original document rather than from this document's own text.
/// </para>
/// </remarks>
public abstract class SourceDocument
{
    /// <summary>
    /// Gets the document's identifier. This is typically
    /// a path.
    /// </summary>
    /// <returns>The document's identifier.</returns>
    public abstract string Identifier { get; }

    /// <summary>
    /// Gets the document's length, in characters.
    /// </summary>
    /// <returns>The document's length.</returns>
    public abstract int Length { get; }

    /// <summary>
    /// Opens this source document at the given offset.
    /// </summary>
    /// <param name="offset">
    /// The offset to open the source document at.
    /// </param>
    /// <returns>A text reader.</returns>
    public abstract TextReader Open(int offset);

    /// <summary>
    /// Gets the diagnostic display position that corresponds to a
    /// particular offset in this document.
    /// </summary>
    /// <param name="offset">The offset to a character in the document.</param>
    /// <returns>A diagnostic display position.</returns>
    public abstract LineAndColumnPosition GetPosition(int offset);

    /// <summary>
    /// Resolves a span in this document to its original source coverage.
    /// </summary>
    /// <param name="start">The zero-based start offset within this document.</param>
    /// <param name="length">The span length within this document.</param>
    /// <returns>The resolved original source coverage.</returns>
    public abstract ResolvedSourceSpan ResolveSpan(int start, int length);

    /// <summary>
    /// Gets a span of text in the document.
    /// </summary>
    /// <param name="offset">
    /// The offset of the first character to read.
    /// </param>
    /// <param name="length">
    /// The number of characters to read.
    /// </param>
    /// <returns>A span of text.</returns>
    public virtual string GetText(int offset, int length)
    {
        var buffer = new char[length];
        using (var reader = Open(offset))
        {
            reader.Read(buffer, 0, length);
        }
        return new StringBuilder().Append(buffer).ToString();
    }
}
