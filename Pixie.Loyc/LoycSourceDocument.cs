﻿using System;
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
        }

        private ISourceFile source;

        /// <inheritdoc/>
        public override string Identifier => source.FileName;

        /// <inheritdoc/>
        public override int Length => source.Text.Count;

        /// <inheritdoc/>
        public override GridPosition GetGridPosition(int offset)
        {
            var linePos = source.IndexToLine(offset);
            return new GridPosition(linePos.Line - 1, linePos.PosInLine - 1);
        }

        /// <inheritdoc/>
        public override int GetLineOffset(int lineIndex)
        {
            if (lineIndex <= 0)
                return 0;
            else if (lineIndex >= source.IndexToLine(source.Text.Count - 1).Line)
                return Length;
            else
                return source.LineToIndex(lineIndex + 1);
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
    }
}

