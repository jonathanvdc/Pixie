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
            : base(identifier, contents)
        {
        }
    }
}
