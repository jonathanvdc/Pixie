# Architecture

Pixie separates what an application wants to say from how a terminal happens to display it.

The normal flow is:

1. Application code creates `LogEntry` values through `ILog` helpers.
2. Log entries carry `MarkupNode` trees.
3. Optional transforms rewrite or enrich markup.
4. `Pixie.Terminal` renderers write markup to a `TerminalBase`.
5. Terminal devices handle ANSI styling, console styling, width, encoding, and degraded rendering.

## Core Library

`Pixie` contains the stable output model:

- `ILog`, `LogEntry`, `RecordingLog`, `NullLog`, and `ThrowingLog`,
- markup nodes in `Pixie.Markup`,
- diagnostic and source-reference types,
- GNU-style option parsing in `Pixie.Options`,
- source documents, spans, and regions in `Pixie.Code`,
- transforms in `Pixie.Transforms`.

Behavior about the meaning or structure of output belongs here.

## Terminal Rendering

`Pixie.Terminal` contains the rendering layer:

- terminal devices,
- layout and wrapping,
- styling,
- capability detection,
- graceful degradation.

Behavior about how markup appears in a terminal belongs here.

## Examples And Docs

Examples demonstrate recommended usage. They should stay small, compile, and show one adoption path clearly.

Documentation should teach tasks and concepts. It should not rely on a copied README as the whole website.
