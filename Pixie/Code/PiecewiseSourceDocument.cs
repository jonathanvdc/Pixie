using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pixie.Code
{
    /// <summary>
    /// Represents a derived source document assembled from ordered source and
    /// generated-text pieces.
    /// </summary>
    /// <remarks>
    /// Piecewise documents are useful for preprocessors, macro expansion,
    /// templating, REPL inputs, generated wrappers, and tests. They avoid storing
    /// one fully synthesized source string as the document's core representation:
    /// text is read from each piece as needed, and source resolution follows each
    /// piece back to its original source span or anchor.
    /// </remarks>
    public sealed class PiecewiseSourceDocument : SourceDocumentView
    {
        private readonly string identifier;
        private readonly IReadOnlyList<SourceDocumentPiece> pieces;
        private readonly int[] pieceStarts;
        private readonly int length;
        private readonly OriginalSourceDocument unknownDocument;

        /// <summary>
        /// Creates a piecewise source document from an identifier and pieces.
        /// </summary>
        /// <param name="identifier">The identifier for the assembled document.</param>
        /// <param name="pieces">The pieces that make up the assembled document.</param>
        public PiecewiseSourceDocument(
            string identifier,
            IEnumerable<SourceDocumentPiece> pieces)
        {
            this.identifier = identifier ?? string.Empty;
            var pieceList = new List<SourceDocumentPiece>(pieces ?? Array.Empty<SourceDocumentPiece>());
            this.pieces = pieceList;
            this.pieceStarts = new int[pieceList.Count];

            int currentStart = 0;
            for (int i = 0; i < pieceList.Count; i++)
            {
                pieceStarts[i] = currentStart;
                currentStart += pieceList[i].Length;
            }

            this.length = currentStart;
            this.unknownDocument = new StringDocument(this.identifier, string.Empty);
        }

        /// <summary>
        /// Gets the ordered pieces that make up this document.
        /// </summary>
        /// <returns>The ordered document pieces.</returns>
        public IReadOnlyList<SourceDocumentPiece> Pieces => pieces;

        /// <summary>
        /// Creates a builder for a piecewise source document.
        /// </summary>
        /// <param name="identifier">The identifier for the assembled document.</param>
        /// <returns>A piecewise source document builder.</returns>
        public static PiecewiseSourceDocumentBuilder Create(string identifier)
        {
            return new PiecewiseSourceDocumentBuilder(identifier);
        }

        /// <inheritdoc/>
        public override string Identifier => identifier;

        /// <inheritdoc/>
        public override int Length => length;

        /// <inheritdoc/>
        public override TextReader Open(int offset)
        {
            if (offset < 0 || offset > Length)
            {
                throw new ArgumentException("offset is out of bounds.", nameof(offset));
            }

            return new PiecewiseTextReader(this, offset);
        }

        /// <inheritdoc/>
        public override string GetText(int offset, int length)
        {
            if (offset < 0 || offset > Length)
            {
                throw new ArgumentException("offset is out of bounds.", nameof(offset));
            }
            if (length < 0 || offset + length > Length)
            {
                throw new ArgumentException("length is out of bounds.", nameof(length));
            }

            var builder = new StringBuilder(length);
            int remainingStart = offset;
            int remainingLength = length;

            while (remainingLength > 0)
            {
                int pieceIndex = GetPieceIndex(remainingStart);
                var piece = pieces[pieceIndex];
                int pieceStart = pieceStarts[pieceIndex];
                int localStart = remainingStart - pieceStart;
                int readLength = Math.Min(remainingLength, piece.Length - localStart);
                builder.Append(piece.GetText(localStart, readLength));
                remainingStart += readLength;
                remainingLength -= readLength;
            }

            return builder.ToString();
        }

        /// <inheritdoc/>
        public override ResolvedSourceSpan ResolveSpan(int start, int length)
        {
            var clampedStart = Math.Max(0, Math.Min(start, Length));
            var clampedEnd = Math.Max(clampedStart, Math.Min(clampedStart + Math.Max(0, length), Length));

            if (pieces.Count == 0)
            {
                return CreateUnknownResolution();
            }

            if (clampedStart == clampedEnd)
            {
                var pointSpan = ResolvePoint(clampedStart);
                return new ResolvedSourceSpan(pointSpan, new[] { pointSpan });
            }

            var origins = new List<OriginalSourceSpan>();
            foreach (var pieceInfo in GetIntersectingPieces(clampedStart, clampedEnd))
            {
                OriginalSourceSpan origin;
                if (pieceInfo.Piece.TryResolveIntersection(
                    pieceInfo.LocalStart,
                    pieceInfo.LocalLength,
                    out origin))
                {
                    origins.Add(origin);
                }
            }

            if (origins.Count == 0)
            {
                var pointSpan = ResolvePoint(clampedStart);
                return new ResolvedSourceSpan(pointSpan, new[] { pointSpan });
            }

            return new ResolvedSourceSpan(origins[0], origins);
        }

        private OriginalSourceSpan ResolvePoint(int offset)
        {
            if (pieces.Count == 0)
            {
                return CreateUnknownSpan();
            }

            int pieceIndex = GetPieceIndex(Math.Min(offset, Math.Max(Length - 1, 0)));
            var piece = pieces[pieceIndex];
            int localOffset = Math.Max(0, Math.Min(offset - pieceStarts[pieceIndex], piece.Length));
            var point = piece.ResolvePoint(localOffset);
            if (point.Document == null)
            {
                return CreateUnknownSpan();
            }

            return point;
        }

        private ResolvedSourceSpan CreateUnknownResolution()
        {
            var span = CreateUnknownSpan();
            return new ResolvedSourceSpan(span, new[] { span });
        }

        private OriginalSourceSpan CreateUnknownSpan()
        {
            return new OriginalSourceSpan(unknownDocument, 0, 0);
        }

        private IEnumerable<PieceIntersection> GetIntersectingPieces(int start, int end)
        {
            int offset = start;
            while (offset < end)
            {
                int pieceIndex = GetPieceIndex(offset);
                var piece = pieces[pieceIndex];
                int pieceStart = pieceStarts[pieceIndex];
                int localStart = offset - pieceStart;
                int localEnd = Math.Min(piece.Length, end - pieceStart);
                yield return new PieceIntersection(piece, localStart, localEnd - localStart);
                offset = pieceStart + localEnd;
            }
        }

        private int GetPieceIndex(int offset)
        {
            if (pieces.Count == 0)
            {
                throw new InvalidOperationException("The piecewise source document has no pieces.");
            }

            if (offset == Length)
            {
                offset = Math.Max(0, offset - 1);
            }

            int lo = 0;
            int hi = pieceStarts.Length - 1;
            while (lo < hi)
            {
                int mid = lo + (hi - lo + 1) / 2;
                if (pieceStarts[mid] <= offset)
                {
                    lo = mid;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return lo;
        }

        private struct PieceIntersection
        {
            public PieceIntersection(
                SourceDocumentPiece piece,
                int localStart,
                int localLength)
            {
                Piece = piece;
                LocalStart = localStart;
                LocalLength = localLength;
            }

            public SourceDocumentPiece Piece { get; }

            public int LocalStart { get; }

            public int LocalLength { get; }
        }

        private sealed class PiecewiseTextReader : TextReader
        {
            private readonly PiecewiseSourceDocument document;
            private int position;

            public PiecewiseTextReader(PiecewiseSourceDocument document, int position)
            {
                this.document = document;
                this.position = position;
            }

            public override int Peek()
            {
                if (position >= document.Length)
                {
                    return -1;
                }

                return ReadCharacter(position);
            }

            public override int Read()
            {
                if (position >= document.Length)
                {
                    return -1;
                }

                var result = ReadCharacter(position);
                position++;
                return result;
            }

            public override int Read(char[] buffer, int index, int count)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }
                if (index < 0 || count < 0 || index + count > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                int totalRead = 0;
                while (count > 0 && position < document.Length)
                {
                    int pieceIndex = document.GetPieceIndex(position);
                    var piece = document.pieces[pieceIndex];
                    int pieceStart = document.pieceStarts[pieceIndex];
                    int localStart = position - pieceStart;
                    int readLength = Math.Min(count, piece.Length - localStart);
                    var text = piece.GetText(localStart, readLength);
                    text.CopyTo(0, buffer, index, readLength);

                    position += readLength;
                    index += readLength;
                    count -= readLength;
                    totalRead += readLength;
                }

                return totalRead;
            }

            private int ReadCharacter(int offset)
            {
                int pieceIndex = document.GetPieceIndex(offset);
                var piece = document.pieces[pieceIndex];
                int localOffset = offset - document.pieceStarts[pieceIndex];
                return piece.GetText(localOffset, 1)[0];
            }
        }
    }
}
