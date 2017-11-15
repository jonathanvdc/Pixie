using System;
using System.IO;
using System.Text;

namespace Pixie.Code
{
    /// <summary>
    /// A document of source code that is stored as a string
    /// </summary>
    public sealed class StringDocument : SourceDocument
    {
        /// <summary>
        /// Creates a string document from an identifier and a contents
        /// string.
        /// </summary>
        /// <param name="identifier">The document's identifier.</param>
        /// <param name="contents">The document's contents.</param>
        public StringDocument(string identifier, string contents)
        {
            this.ident = identifier;
            this.Contents = contents;
        }

        private string ident;

        /// <summary>
        /// Gets the string that defines this string document's contents.
        /// </summary>
        /// <returns>The document's contents string.</returns>
        public string Contents { get; private set; }

        /// <inheritdoc/>
        public override string Identifier => ident;

        /// <inheritdoc/>
        public override int Length => Contents.Length;

        /// <inheritdoc/>
        public override TextReader Open(int offset)
        {
            var reader = new StringReader(Contents);
            if (offset > 0)
            {
                int bufSize = 1024;
                var buffer = new char[bufSize];
                while (offset > 0)
                {
                    var readCount = Math.Min(offset, bufSize);
                    reader.Read(buffer, 0, readCount);
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
}
