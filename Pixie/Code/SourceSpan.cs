using System;

namespace Pixie.Code
{
    /// <summary>
    /// Represents a source span backed by a document-relative character offset and length.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Start"/> and <see cref="Length"/> are relative to the owning
    /// <see cref="SourceDocument"/>'s text. The owning document may be original source text or a
    /// derived source view such as preprocessed text.
    /// </para>
    /// <para>
    /// Line, column, file name, and original source spans are resolved on demand through the owning
    /// document. This keeps tokens compact while allowing mapped documents to report diagnostics
    /// against user-authored source.
    /// </para>
    /// <para>
    /// An <em>unknown</em> location is represented by the <see langword="default"/> value (i.e., a
    /// <see langword="null"/> document). Use <see cref="Unknown"/> to obtain one explicitly.
    /// </para>
    /// </remarks>
    public struct SourceSpan
    {
        /// <summary>
        /// Creates a source span from a document, a start offset and a span length.
        /// </summary>
        /// <param name="document">
        /// The source document this span is a part of.
        /// </param>
        /// <param name="start">
        /// The offset of the first character in the span.
        /// </param>
        /// <param name="length">
        /// The number of characters in the span.
        /// </param>
        public SourceSpan(
            SourceDocument document,
            int start,
            int length)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if (start < 0 || start > document.Length)
            {
                throw new ArgumentException("start is out of bounds.", nameof(start));
            }
            if (length < 0 || start + length > document.Length)
            {
                throw new ArgumentException("length is out of bounds.", nameof(length));
            }

            this = default(SourceSpan);
            this.Document = document;
            this.Start = start;
            this.Length = length;
        }

        /// <summary>
        /// Gets the document to which this span of source code refers.
        /// </summary>
        /// <returns>The source document.</returns>
        public SourceDocument Document { get; private set; }

        /// <summary>
        /// Gets the offset of the first character in this span.
        /// </summary>
        /// <returns>The offset of the first character in this span.</returns>
        public int Start { get; private set; }

        /// <summary>
        /// Gets this span's length, in characters.
        /// </summary>
        /// <returns>The length of this span.</returns>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the exclusive end offset of this span.
        /// </summary>
        /// <returns>The exclusive end offset of this span.</returns>
        public int End => Start + Length;

        /// <summary>
        /// Gets a value indicating whether this span is backed by a source document.
        /// </summary>
        /// <returns><c>true</c> if this span is known; otherwise, <c>false</c>.</returns>
        public bool IsKnown => Document != null;

        /// <summary>
        /// Gets the diagnostic display position of the span start.
        /// </summary>
        /// <returns>The diagnostic display position.</returns>
        public SourcePosition Position =>
            IsKnown ? Document.GetPosition(Start) : SourcePosition.Unknown;

        /// <summary>
        /// Gets this span's contents as text.
        /// </summary>
        /// <returns>The span's contents.</returns>
        public string Text => IsKnown ? Document.GetText(Start, Length) : string.Empty;

        /// <summary>
        /// Resolves this span to original source coverage.
        /// </summary>
        /// <returns>The resolved original source coverage.</returns>
        public ResolvedSourceSpan Resolve()
        {
            return IsKnown ? Document.ResolveSpan(Start, Length) : null;
        }

        /// <summary>
        /// Gets an unknown source span.
        /// </summary>
        public static SourceSpan Unknown => default(SourceSpan);

        /// <summary>
        /// Merges two source spans into a single span that covers both.
        /// </summary>
        /// <param name="first">The first span.</param>
        /// <param name="second">The second span.</param>
        /// <returns>The merged source span.</returns>
        public static SourceSpan Merge(SourceSpan first, SourceSpan second)
        {
            if (!first.IsKnown)
            {
                return second;
            }
            if (!second.IsKnown)
            {
                return first;
            }
            if (!ReferenceEquals(first.Document, second.Document))
            {
                return first;
            }

            int start = Math.Min(first.Start, second.Start);
            int end = Math.Max(first.End, second.End);
            return new SourceSpan(first.Document, start, end - start);
        }
    }
}
