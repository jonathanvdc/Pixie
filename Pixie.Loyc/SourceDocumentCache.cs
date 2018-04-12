using System.Collections.Generic;
using System.Threading;
using Pixie.Code;

namespace Pixie.Loyc
{
    /// <summary>
    /// A container for a set of source documents that can be addressed
    /// by file name. This data structure is used by
    /// <see cref="Pixie.Loyc.PixieMessageSink"/> instances to translate
    /// <see cref="global::Loyc.Syntax.SourcePos"/> values to caret
    /// diagnostics.
    /// </summary>
    public sealed class SourceDocumentCache
    {
        /// <summary>
        /// Creates an empty source document cache.
        /// </summary>
        public SourceDocumentCache()
        {
            this.docs = new Dictionary<string, SourceDocument>();
            this.rwLock = new ReaderWriterLockSlim();
        }

        private Dictionary<string, SourceDocument> docs;
        private ReaderWriterLockSlim rwLock;

        /// <summary>
        /// Gets the <see cref="Pixie.Loyc.SourceDocumentCache"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the document to retrieve.</param>
        public SourceDocument this[string name]
        {
            get
            {
                SourceDocument result;
                try
                {
                    rwLock.EnterReadLock();
                    result = docs[name];
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
                return result;
            }
        }

        /// <summary>
        /// Tries to get a <see cref="Pixie.Code.SourceDocument"/> with a particular name.
        /// </summary>
        /// <returns><c>true</c>, if the document was successfully retrieved, <c>false</c> otherwise.</returns>
        /// <param name="name">The name of the document to retrieve.</param>
        /// <param name="result">The location to store the resulting source document in.</param>
        public bool TryGetDocument(string name, out SourceDocument result)
        {
            bool foundDocument;
            try
            {
                rwLock.EnterReadLock();
                foundDocument = docs.TryGetValue(name, out result);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
            return foundDocument;
        }


        /// <summary>
        /// Adds a particular document to this source document cache.
        /// </summary>
        /// <param name="document">The document to add.</param>
        public void Add(SourceDocument document)
        {
            rwLock.EnterWriteLock();
            try
            {
                docs[document.Identifier] = document;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }
}