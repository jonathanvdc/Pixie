# Source Model

Pixie's source model supports source-aware diagnostics without forcing every source document to expose a line grid.

Use the model this way:

- `SourceDocument` is a readable source buffer.
- `OriginalSourceDocument` owns final diagnostic coordinates.
- `StringDocument` is a string-backed original document.
- `SourceDocumentView` and derived documents map generated offsets back to original source.
- `SourceSpan` represents a contiguous known-or-unknown span.
- `SourceRegion` represents a possibly sparse set of highlighted characters.
- `LineAndColumnPosition` uses one-based diagnostic display coordinates.

## Offset Rules

Internal offsets are zero-based and half-open:

- `Start` is the first character offset,
- `Length` is the number of characters,
- `End` is one past the final character.

Display coordinates are one-based at API boundaries:

- `Line`,
- `Column`.

## Reading Text

`Open(int offset)` should remain useful for large documents and avoid unnecessary large string allocations. `GetText(offset, length)` may allocate the exact requested text because the caller explicitly asked for that string.

## Derived Documents

Generated or preprocessed documents should resolve spans back to original user-authored source. Diagnostics should point at what the user wrote, not at generated intermediate text.

Use `SourceSpan` for contiguous locations and `SourceRegion` for sparse highlighting. Preserve unknown spans when APIs can reasonably lack source information.
