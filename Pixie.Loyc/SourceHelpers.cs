using System;
using Pixie.Code;
using Loyc.Syntax;

namespace Pixie.Loyc
{
    /// <summary>
    /// Helper methods that bride the divide between Loyc and Pixie
    /// source source references.
    /// </summary>
    public static class SourceHelpers
    {
        /// <summary>
        /// Creates a Pixie source region that is equivalent to a
        /// Loyc source range.
        /// </summary>
        /// <param name="range">A source range.</param>
        /// <returns>A source region.</returns>
        public static SourceRegion ToSourceRegion(this SourceRange range)
        {
            return new SourceRegion(ToSourceSpan(range));
        }

        /// <summary>
        /// Creates a Pixie source span that is equivalent to a
        /// Loyc source range.
        /// </summary>
        /// <param name="range">A source range.</param>
        /// <returns>A source span.</returns>
        public static SourceSpan ToSourceSpan(this SourceRange range)
        {
            var document = ToSourceDocument(range.Source);

            var clampedStartOffset = Math.Max(0, Math.Min(document.Length - 1, range.StartIndex));
            var clampedEndOffset = Math.Max(0, Math.Min(document.Length - 1, range.EndIndex));
            return new SourceSpan(
                document,
                clampedStartOffset,
                clampedEndOffset - clampedStartOffset);
        }

        /// <summary>
        /// Wraps a Loyc source file in a Pixie source document.
        /// </summary>
        /// <param name="sourceFile">A Loyc source file to wrap.</param>
        /// <returns>A Pixie source document.</returns>
        public static SourceDocument ToSourceDocument(this ISourceFile sourceFile)
        {
            return new LoycSourceDocument(sourceFile);
        }
    }
}
