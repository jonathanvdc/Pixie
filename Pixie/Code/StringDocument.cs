using System.IO;

namespace Pixie.Code
{
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
            return new StringReader(GetText(offset, Length - offset));
        }

        /// <inheritdoc/>
        public override string GetText(int offset, int length)
        {
            return Contents.Substring(offset, length);
        }
    }
}
