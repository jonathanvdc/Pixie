# Pixie Agent Guide

This repository contains Pixie, a C# library for polished command-line output,
compiler-style diagnostics, help text, and GNU-style option parsing.

- `Pixie`
  Core library: logging abstractions, markup nodes, diagnostics, option parsing,
  source documents/spans/regions, text diffs, name suggestions, and transforms.
- `Pixie.Terminal`
  Terminal devices and renderers that turn markup trees into terminal output with
  wrapping, styling, alignment, and graceful degradation.
- `Tests`
  NUnit test suite covering core behavior, rendering, diagnostics, option
  parsing, and source references.
- `Examples`
  Small executable examples for documentation, screenshots, and user-facing
  scenarios.
- `docs`
  User documentation and generated/rendered images used by the README and site.

## Architecture

The intended flow for user-facing output is:

1. Application code creates `LogEntry` values through `ILog` helpers.
2. Log entries carry `MarkupNode` trees.
3. Optional transforms rewrite or enrich markup, for example diagnostic
   extraction.
4. `Pixie.Terminal` renderers write markup to a `TerminalBase`.
5. Terminal devices handle concrete output details such as ANSI styling,
   console styling, width, and degraded rendering.

Prefer implementing behavior at the earliest correct layer.

- If behavior is about the structure or meaning of output, implement it in
  `Pixie`.
- If behavior is about how markup appears in a terminal, implement it in
  `Pixie.Terminal`.
- If behavior is only a usage demonstration, keep it in `Examples` or `docs`
  rather than adding library API.

Keep markup semantic. Do not encode terminal escape behavior, line wrapping, or
device assumptions into markup nodes.

## Core Library Design

`Pixie` is the stable center of the library. Important areas:

- `ILog`, `LogEntry`, `NullLog`, `RecordingLog`, and `ThrowingLog`
  define and exercise application-facing logging behavior.
- `MarkupNode` and `Pixie.Markup`
  define the structured output tree.
- `Pixie.Transforms`
  rewrites or extracts meaning from markup trees.
- `Pixie.Options`
  parses GNU-style command lines and generates matching help output.
- `Pixie.Code`
  models source documents, source spans, source regions, and diagnostic
  positions.

When adding public API:

- keep the common path small and easy to discover
- preserve composability through existing markup and log abstractions
- add XML documentation that explains what the API is for and how it composes
  with the rest of Pixie
- add tests that describe both ordinary behavior and boundary behavior

## Source Documents And Diagnostics

The source model is intentionally split by responsibility:

- `SourceDocument`
  A readable source buffer with large-document-friendly text access through
  `Open(int offset)`, plus targeted `GetText(int offset, int length)` reads.
- `OriginalSourceDocument`
  Base type for user-authored source documents that own final diagnostic
  coordinates, original-source provenance, and source line extraction.
- `StringDocument`
  String-backed original source document. Keep text reads efficient: do not make
  `Open(int offset)` allocate the entire remaining document as a substring.
- `SourceDocumentView`
  Base type for generated, preprocessed, or otherwise derived documents that map
  offsets back to original source.
- `PiecewiseSourceDocument`
  Source document view assembled from source-backed and generated-text pieces.
  It exposes assembled text offsets for parsers, but diagnostic positions should
  resolve back to original documents.
- `SourceSpan`
  A contiguous known-or-unknown span in a source document.
- `SourceRegion`
  A possibly sparse set of characters in one document, used for highlighted
  source rendering.
- `LineAndColumnPosition`
  One-based diagnostic display coordinates.
- `SourceLine`
  A display line in an `OriginalSourceDocument`. Do not add line-grid APIs to
  arbitrary `SourceDocument` implementations.
- `ResolvedSourceSpan` and `OriginalSourceSpan`
  Original-source provenance for spans in original or derived documents.

Follow these rules when changing source behavior:

- `Open(int offset)` should remain useful for large documents and should avoid
  unnecessary large string allocations.
- `GetText(offset, length)` may allocate exactly the requested text; callers
  explicitly asked for that string.
- Use `SourceSpan` for contiguous source locations and `SourceRegion` for sparse
  highlighting.
- Keep display coordinates one-based at API boundaries.
- Keep offsets zero-based and half-open internally: `Start`, `Length`, `End`.
- For derived documents, implement `ResolveSpan` so diagnostics point at
  user-authored source, not generated text.
- Do not expose assembled-document line counts or line offsets for source views.
  A piecewise or generated document's line grid is an implementation detail and
  is usually the wrong diagnostic coordinate space.
- Preserve unknown spans through APIs that can reasonably lack source
  information.

When rendering source diagnostics, prefer deriving file, line, and column from
`SourceSpan.Position` or `SourceDocument.GetPosition(...)` rather than
recomputing coordinates locally.

## Markup And Rendering

Markup nodes describe output intent. Renderers decide presentation.

Good examples:

- `Text` carries text.
- `Sequence` carries ordered markup.
- `WrapBox`, `AlignBox`, `IndentBox`, and `PrefixBox` describe layout intent.
- `ColorSpan` and `DecorationSpan` describe styling intent.
- `HighlightedSource` describes a source highlight, not terminal caret drawing.

Renderer guidance:

- Put terminal-width, wrapping, styling, and degradation behavior in
  `Pixie.Terminal`.
- Keep renderers deterministic and covered by rendering tests.
- Prefer adding a renderer for a new markup node over teaching unrelated
  renderers about special cases.
- Be careful with whitespace. Many tests intentionally cover trailing spaces,
  empty lines, tabs, wrapping, and caret visibility.
- Do not assume ANSI styling is available. Pixie should degrade gracefully on
  plain text terminals.

## Option Parsing

`Pixie.Options` owns GNU-style parsing and generated help.

When changing option parsing:

- keep parsing definitions and help generation in sync
- preserve `OptionParseResult` as the place callers inspect success, handled
  help/version exits, exit codes, and typed values
- report user-facing parse problems through `ILog`
- add tests for parse results and rendered/logged feedback
- keep suggestions and diagnostics readable for real CLI users, not only correct
  as data structures

`WasHandled` is for parser-managed early exits such as help/version. Parse
failures should be represented by `IsSuccess == false`.

## Tests

The main test project is `Tests/Tests.csproj`.

Important test areas:

- `StringDocumentTests`, `SourceSpanTests`, `SourceRegionTests`,
  `SourceDocumentViewTests`
  Source model and source provenance behavior.
- `CaretDiagnosticTests`
  Explicit diagnostic markup and source-highlight behavior.
- `RenderTests`, `RenderingBehaviorTests`, `TerminalDeviceTests`
  Terminal rendering and device behavior.
- `GnuOptionSetParserTests`, `OptionFormattingTests`
  Option parsing and help formatting.
- `MarkupNodeBehaviorTests`, `CoreValueTests`, `LogBehaviorTests`
  Core object and logging behavior.

When changing behavior:

- add narrow unit tests near the changed area
- add rendering tests when output shape changes
- update examples/docs when user-facing behavior or recommended usage changes
- keep tests readable; expected output should explain the intended user
  experience

## Build And Validation

Preferred validation commands:

```bash
dotnet build Pixie.sln
dotnet test Pixie.sln
dotnet test Tests/Tests.csproj
dotnet run --project Examples/CaretDiagnostics/CaretDiagnostics.csproj
dotnet run --project Examples/ParseOptions/ParseOptions.csproj
dotnet run --project Examples/PrintHelp/PrintHelp.csproj
```

Use the full solution test command before finishing changes that touch public
API, source diagnostics, rendering, option parsing, or terminal devices.

If only docs changed, build/test may be unnecessary, but still inspect links,
examples, and code snippets carefully.

## Documentation Policy

Good documentation is part of the implementation in this repository.

- Add XML documentation for public APIs.
- Also document non-public types and members when they carry behavior,
  invariants, caching rules, source mapping, rendering decisions, or other logic
  a maintainer would otherwise need to reverse engineer.
- Optimize comments for reader understanding, not ceremony. Explain
  responsibilities, data flow, assumptions, and why an algorithm is structured a
  certain way.
- Add inline comments for non-obvious control flow, subtle terminal behavior,
  source provenance decisions, lazy caching, line/column math, degraded
  rendering, and compatibility choices.
- Do not add comments that merely restate the code line-by-line.
- When touching older code with weak documentation, improve it as you go.
- Keep README/docs examples aligned with actual API names and behavior.
- If a behavior exists to preserve CLI usability, terminal compatibility, or
  source-diagnostic correctness, document that motivation near the code or in
  tests.

For user docs:

- Prefer small, complete examples over abstract descriptions.
- Say when output goes to standard error versus standard output.
- Keep parsing and help examples in sync with the real option APIs.
- Treat screenshots and rendered examples as user-facing artifacts. If behavior
  changes, update or regenerate the affected documentation assets.

## Examples And Docs

Examples are part of the public contract. They should compile and demonstrate
recommended usage.

Use examples this way:

- `Examples/CaretDiagnostics`
  Source spans, regions, and caret diagnostics.
- `Examples/FormattedList`
  Layout, wrapping, and terminal configuration.
- `Examples/ParseOptions`
  Parsing and user-facing parse feedback.
- `Examples/PrintHelp`
  Generated help output.
- `Examples/SimpleErrorMessage`
  Minimal diagnostic-style output.

When changing example-visible behavior, update the corresponding docs under
`docs/` and README sections.

## Editing Guidance

- Do not edit `bin/`, `obj/`, `TestResults/`, or generated build artifacts.
- Do not keep copied reference projects or temporary source imports in the repo.
- Use `rg` to search and prefer focused changes over broad rewrites.
- Keep public API names consistent: source offsets use `Start`, `Length`, and
  `End`; diagnostic coordinates use `Line` and `Column`.
- Avoid introducing dependencies for simple rendering, parsing, or formatting
  behavior unless they clearly improve the library.
- Preserve multi-targeting behavior for `netstandard2.0` and `net10.0`.
- Be careful with allocation-sensitive APIs, especially source text reading and
  terminal rendering loops.

## If Unsure

Ask:

1. Is this about what the application wants to say? Put it in core markup,
   logging, options, diagnostics, or source modeling.
2. Is this about how output appears on a terminal? Put it in `Pixie.Terminal`.
3. Is this about teaching users? Put it in `Examples` or `docs`.

When in doubt, choose the layer that can express the behavior without knowing
about lower-level rendering or higher-level application details.
