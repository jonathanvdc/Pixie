using System;
using System.Collections.Generic;

namespace Pixie.Code
{
    /// <summary>
    /// Describes the original source coverage for a span in a source document.
    /// </summary>
    public sealed class ResolvedSourceSpan
    {
        /// <summary>
        /// Creates resolved source span information.
        /// </summary>
        /// <param name="primarySpan">The primary span for classic diagnostic display.</param>
        /// <param name="originSpans">All original source spans covered by the resolved span.</param>
        public ResolvedSourceSpan(
            OriginalSourceSpan primarySpan,
            IReadOnlyList<OriginalSourceSpan> originSpans)
        {
            this.PrimarySpan = primarySpan;
            this.OriginSpans = originSpans ?? Array.Empty<OriginalSourceSpan>();
        }

        /// <summary>
        /// Gets the best single span for classic diagnostic display.
        /// </summary>
        /// <returns>The primary original source span.</returns>
        public OriginalSourceSpan PrimarySpan { get; private set; }

        /// <summary>
        /// Gets all original source spans covered by the resolved span.
        /// </summary>
        /// <returns>The original source spans covered by the resolved span.</returns>
        public IReadOnlyList<OriginalSourceSpan> OriginSpans { get; private set; }
    }
}
