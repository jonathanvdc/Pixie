using System;
using System.Collections.Generic;

namespace Pixie.Code;

/// <summary>
/// Represents the original source coverage for a span in a parsed source document.
/// </summary>
/// <remarks>
/// A contiguous span in a mapped document may originate from several disjoint spans, and
/// eventually from several source files. <see cref="PrimarySpan"/> is the single best span
/// for classic diagnostic display, while <see cref="OriginSpans"/> preserves the complete
/// original-source coverage for richer diagnostics.
/// </remarks>
/// <remarks>
/// Creates resolved source span information.
/// </remarks>
/// <param name="primarySpan">The primary span for classic diagnostic display.</param>
/// <param name="originSpans">All original source spans covered by the resolved span.</param>
public sealed class ResolvedSourceSpan(
    OriginalSourceSpan primarySpan,
    IReadOnlyList<OriginalSourceSpan> originSpans)
{

    /// <summary>
    /// Gets the best single span for classic diagnostic display.
    /// </summary>
    /// <returns>The primary original source span.</returns>
    public OriginalSourceSpan PrimarySpan { get; } = primarySpan;

    /// <summary>
    /// Gets all original source spans covered by the resolved span.
    /// </summary>
    /// <returns>The original source spans covered by the resolved span.</returns>
    public IReadOnlyList<OriginalSourceSpan> OriginSpans { get; } = originSpans ?? Array.Empty<OriginalSourceSpan>();
}
