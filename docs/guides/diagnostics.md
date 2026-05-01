# Diagnostics

Pixie diagnostics are for messages that have an origin, a severity, a title, and optional structured body content. They are useful for compiler-style output such as:

```text
file.cs:12:4: error: expected expression
```

Diagnostics often pair a header with a highlighted source snippet.

## Render A Source Diagnostic

```cs
using Pixie;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

var document = new StringDocument("file.cs", source);
var span = new SourceSpan(document, offset, length);
var region = new SourceRegion(span);
var highlightedSource = new HighlightedSource(region, region);

log.ErrorDiagnostic(
    new SourceReference(highlightedSource.HighlightedSpan),
    "expected expression",
    highlightedSource);
```

`HighlightedSource` renders the source snippet and caret markers. `SourceReference` gives the diagnostic header its file, line, and column information.

## Keep Diagnostics Explicit

Logging a `HighlightedSource` by itself renders the snippet, but it does not automatically turn the entry into a full diagnostic. Use the diagnostic helpers when you want the header too.

That separation lets one application mix diagnostics, help output, progress messages, and ordinary status text through the same log without surprising conversions.

## Source Provenance

Diagnostics should resolve positions through Pixie's source model instead of recomputing line and column information locally. For generated or preprocessed text, source views can map spans back to original user-authored documents.

Read [Source Model](../concepts/source-model.md) for the coordinate rules.

## See It In Context

The [CaretDiagnostics example](https://github.com/jonathanvdc/Pixie/blob/master/Examples/CaretDiagnostics/Program.cs) shows source spans, focus regions, diagnostic headers, and renderer customization.
