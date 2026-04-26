using System;

namespace Pixie.Code
{
    /// <summary>
    /// Represents one piece of text in a <see cref="PiecewiseSourceDocument"/>.
    /// </summary>
    /// <remarks>
    /// A piece can either expose text copied from an existing <see cref="SourceSpan"/>
    /// or literal generated text. Literal text may still have an anchor span so
    /// diagnostics in generated text can point back to the user-authored source
    /// that caused the text to exist.
    /// </remarks>
    public sealed class SourceDocumentPiece
    {
        private SourceDocumentPiece(
            string text,
            SourceSpan origin,
            bool readsFromOrigin)
        {
            this.Text = text;
            this.Origin = origin;
            this.ReadsFromOrigin = readsFromOrigin;
            this.Length = readsFromOrigin ? origin.Length : text.Length;
        }

        /// <summary>
        /// Gets the literal text for this piece, or <see langword="null"/> when
        /// this piece reads its text directly from <see cref="Origin"/>.
        /// </summary>
        /// <returns>The literal text, or <see langword="null"/>.</returns>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the source span this piece came from or is anchored to.
        /// </summary>
        /// <returns>The source span this piece came from or is anchored to.</returns>
        public SourceSpan Origin { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this piece reads its text from
        /// <see cref="Origin"/> instead of storing literal text.
        /// </summary>
        /// <returns><c>true</c> when text is read from the origin span.</returns>
        public bool ReadsFromOrigin { get; private set; }

        /// <summary>
        /// Gets the length of this piece in the assembled document.
        /// </summary>
        /// <returns>The length of this piece.</returns>
        public int Length { get; private set; }

        /// <summary>
        /// Creates a piece whose text and source mapping both come from a source span.
        /// </summary>
        /// <param name="origin">The source span to copy into the assembled document.</param>
        /// <returns>A source-backed piece.</returns>
        public static SourceDocumentPiece FromSource(SourceSpan origin)
        {
            if (!origin.IsKnown)
            {
                throw new ArgumentException(
                    "Source-backed document pieces require a known source span.",
                    nameof(origin));
            }

            return new SourceDocumentPiece(null, origin, true);
        }

        /// <summary>
        /// Creates a generated text piece with no known source provenance.
        /// </summary>
        /// <param name="text">The generated text.</param>
        /// <returns>A literal text piece.</returns>
        public static SourceDocumentPiece FromText(string text)
        {
            return new SourceDocumentPiece(text ?? string.Empty, SourceSpan.Unknown, false);
        }

        /// <summary>
        /// Creates a generated text piece anchored to a source span for diagnostics.
        /// </summary>
        /// <param name="text">The generated text.</param>
        /// <param name="anchor">The span that caused the generated text to exist.</param>
        /// <returns>An anchored literal text piece.</returns>
        public static SourceDocumentPiece FromText(string text, SourceSpan anchor)
        {
            return new SourceDocumentPiece(text ?? string.Empty, anchor, false);
        }

        internal string GetText(int start, int length)
        {
            if (ReadsFromOrigin)
            {
                return Origin.Document.GetText(Origin.Start + start, length);
            }

            return Text.Substring(start, length);
        }

        internal OriginalSourceSpan ResolvePoint(int offset)
        {
            if (!Origin.IsKnown)
            {
                return default(OriginalSourceSpan);
            }

            if (ReadsFromOrigin)
            {
                var resolved = Origin.Document.ResolveSpan(Origin.Start + offset, 0);
                return resolved.PrimarySpan;
            }

            return ResolveAnchorPoint();
        }

        internal bool TryResolveIntersection(
            int start,
            int length,
            out OriginalSourceSpan span)
        {
            span = default(OriginalSourceSpan);
            if (!Origin.IsKnown)
            {
                return false;
            }

            if (!ReadsFromOrigin)
            {
                span = ResolveAnchorPoint();
                return true;
            }

            var resolved = Origin.Document.ResolveSpan(Origin.Start + start, length);
            span = resolved.PrimarySpan;
            return true;
        }

        private OriginalSourceSpan ResolveAnchorPoint()
        {
            var resolved = Origin.Document.ResolveSpan(Origin.Start, 0);
            return resolved.PrimarySpan;
        }
    }
}
