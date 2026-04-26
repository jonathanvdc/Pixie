using System;
using System.Collections.Generic;

namespace Pixie.Code;

/// <summary>
/// Represents a user-authored source document and owns its display coordinate mapping.
/// </summary>
/// <remarks>
/// Original documents are the final diagnostic coordinate space. Mapped or preprocessed
/// documents ultimately resolve their locations to <see cref="OriginalSourceSpan"/> values
/// backed by instances of this type.
/// </remarks>
public abstract class OriginalSourceDocument : SourceDocument
{
    private readonly string identifier;
    private readonly Lazy<List<int>> lazyLineOffsets;

    /// <summary>
    /// Creates an original source document from an identifier and contents string.
    /// </summary>
    /// <param name="identifier">The document's identifier.</param>
    public OriginalSourceDocument(string identifier)
    {
        this.identifier = identifier;
        this.lazyLineOffsets = new Lazy<List<int>>(ComputeLineOffsets);
    }

    private IReadOnlyList<int> lineOffsets => lazyLineOffsets.Value;

    /// <inheritdoc/>
    public override string Identifier => identifier;

    /// <inheritdoc/>
    public int LineCount => lineOffsets.Count;

    /// <inheritdoc/>
    public override LineAndColumnPosition GetPosition(int offset)
    {
        if (offset < 0)
        {
            offset = 0;
        }
        else if (offset > Length)
        {
            offset = Length;
        }

        int lo = 0;
        int hi = lineOffsets.Count - 1;
        while (lo < hi)
        {
            int mid = lo + (hi - lo + 1) / 2;
            if (lineOffsets[mid] <= offset)
            {
                lo = mid;
            }
            else
            {
                hi = mid - 1;
            }
        }

        return new LineAndColumnPosition(
            Identifier,
            lo + 1,
            offset - lineOffsets[lo] + 1);
    }

    /// <summary>
    /// Tries to get a display line by its zero-based line index.
    /// </summary>
    /// <param name="lineIndex">The zero-based line index.</param>
    /// <param name="line">The source line, if it exists.</param>
    /// <returns><c>true</c> if the line exists; otherwise, <c>false</c>.</returns>
    public bool TryGetLine(int lineIndex, out SourceLine line)
    {
        if (lineIndex < 0 || lineIndex >= lineOffsets.Count)
        {
            line = default(SourceLine);
            return false;
        }

        int lineStart = lineOffsets[lineIndex];
        int lineEnd = lineIndex + 1 < lineOffsets.Count
            ? lineOffsets[lineIndex + 1]
            : Length;

        while (lineEnd > lineStart)
        {
            var last = GetText(lineEnd - 1, 1)[0];
            if (last != '\r' && last != '\n')
            {
                break;
            }

            lineEnd--;
        }

        line = new SourceLine(this, lineIndex, lineStart, lineEnd - lineStart);
        return true;
    }

    /// <summary>
    /// Gets the display line that contains a document offset.
    /// </summary>
    /// <param name="offset">The document offset.</param>
    /// <returns>The source line that contains the offset.</returns>
    public SourceLine GetLineByOffset(int offset)
    {
        var position = GetPosition(offset);
        SourceLine line;
        if (!TryGetLine(position.Line - 1, out line))
        {
            throw new ArgumentException("offset is out of bounds.", nameof(offset));
        }

        return line;
    }

    /// <summary>
    /// Tries to convert one-based line and column coordinates to an offset.
    /// </summary>
    /// <param name="line">The one-based line.</param>
    /// <param name="column">The one-based column.</param>
    /// <param name="offset">The resulting offset, if the coordinates are valid.</param>
    /// <returns><c>true</c> if an offset could be computed; otherwise, <c>false</c>.</returns>
    public bool TryGetOffset(int line, int column, out int offset)
    {
        SourceLine sourceLine;
        if (line < 1 || column < 1 || !TryGetLine(line - 1, out sourceLine))
        {
            offset = 0;
            return false;
        }

        offset = sourceLine.Start + column - 1;
        if (offset > sourceLine.End)
        {
            offset = 0;
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override ResolvedSourceSpan ResolveSpan(int start, int length)
    {
        if (start < 0 || start > Length)
        {
            throw new ArgumentException("start is out of bounds.", nameof(start));
        }
        if (length < 0 || start + length > Length)
        {
            throw new ArgumentException("length is out of bounds.", nameof(length));
        }

        var span = new OriginalSourceSpan(this, start, length);
        return new ResolvedSourceSpan(span, new[] { span });
    }

    private List<int> ComputeLineOffsets()
    {
        var results = new List<int>();
        results.Add(0);

        int offset = 0;
        using (var reader = Open(0))
        {
            int value;
            while ((value = reader.Read()) >= 0)
            {
                offset++;
                if ((char)value == '\n')
                {
                    results.Add(offset);
                }
            }
        }

        return results;
    }
}
