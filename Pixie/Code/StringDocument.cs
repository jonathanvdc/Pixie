using System.IO;

namespace Pixie.Code;

/// <summary>
/// A document of source code that is stored as a string.
/// </summary>
public sealed class StringDocument : OriginalSourceDocument
{
    /// <summary>
    /// Creates a string document from an identifier and a contents string.
    /// </summary>
    /// <param name="identifier">The document's identifier.</param>
    /// <param name="contents">The document's contents.</param>
    public StringDocument(string identifier, string contents)
        : base(identifier)
    {
        this.Contents = contents;
    }

    /// <summary>
    /// Gets the string that defines this source document's contents.
    /// </summary>
    /// <returns>The document's contents string.</returns>
    public string Contents { get; private set; }

    /// <inheritdoc/>
    public override int Length => Contents.Length;

    /// <inheritdoc/>
    public override TextReader Open(int offset)
    {
        var reader = new StringReader(Contents);
        if (offset > 0)
        {
            int bufferSize = 1024;
            var buffer = new char[bufferSize];
            while (offset > 0)
            {
                int readCount = System.Math.Min(offset, bufferSize);
                reader.Read(buffer, 0, readCount);
                offset -= readCount;
            }
        }

        return reader;
    }

    /// <inheritdoc/>
    public override string GetText(int offset, int length)
    {
        return Contents.Substring(offset, length);
    }
}
