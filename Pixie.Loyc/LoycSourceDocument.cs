using System;
using Loyc.Syntax;
using Pixie.Code;
using System.IO;

namespace Pixie.Loyc
{
    /// <summary>
    /// A Pixie source document that wraps around a Loyc source document.
    /// </summary>
    internal sealed class LoycSourceDocument : SourceDocument
    {
        public LoycSourceDocument(ISourceFile source)
        {
            this.source = source;
            this.lineCountCache = new Lazy<int>(ComputeLineCount);
            this.originalDocumentCache = new Lazy<OriginalSourceDocument>(CreateOriginalDocument);
        }

        private ISourceFile source;
        private Lazy<int> lineCountCache;
        private Lazy<OriginalSourceDocument> originalDocumentCache;

        /// <inheritdoc/>
        public override string Identifier => source.FileName;

        /// <inheritdoc/>
        public override int Length => source.Text.Count;

        /// <inheritdoc/>
        public override int LineCount => lineCountCache.Value;

        private int ComputeLineCount()
        {
            return originalDocumentCache.Value.LineCount;
        }

        /// <inheritdoc/>
        public override SourcePosition GetPosition(int offset)
        {
            return originalDocumentCache.Value.GetPosition(offset);
        }

        /// <inheritdoc/>
        public override int GetLineOffset(int lineIndex)
        {
            return originalDocumentCache.Value.GetLineOffset(lineIndex);
        }

        /// <inheritdoc/>
        public override string GetText(int offset, int length)
        {
            return source.Text.Slice(offset, length).ToString();
        }

        /// <inheritdoc/>
        public override TextReader Open(int offset)
        {
            // TODO: maybe implement this more efficiently?
            return new StringReader(GetText(offset, Length - offset));
        }

        /// <inheritdoc/>
        public override ResolvedSourceSpan ResolveSpan(int start, int length)
        {
            return originalDocumentCache.Value.ResolveSpan(start, length);
        }

        private OriginalSourceDocument CreateOriginalDocument()
        {
            return new StringDocument(
                source.FileName,
                source.Text.Slice(0, source.Text.Count).ToString());
        }
    }
}
