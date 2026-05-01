# Markup And Layout

Markup nodes are Pixie's structured output tree. They describe intent: text, lists, wrapping, styling, source highlights, and other logical pieces of output.

Renderers decide how those nodes appear in a terminal.

## Common Nodes

| Type | Use it for |
| --- | --- |
| `Text` | Explicit text nodes. |
| `Sequence` | Concatenating inline or block markup. |
| `Paragraph` | A paragraph of text that can wrap. |
| `BulletedList` | Lists with renderer-selected bullets and spacing. |
| `WrapBox` | Wrapped content. |
| `AlignBox` | Horizontally aligned content. |
| `IndentBox` | Indented content. |
| `PrefixBox` | A prefix beside wrapped content. |
| `ColorSpan` | Semantic color styling. |
| `DecorationSpan` | Decorations such as bold or underline. |
| `HighlightedSource` | Source snippets with highlighted regions. |

## Compose Output

```cs
using Pixie.Markup;

var message = new Sequence(
    new Title("Summary"),
    new BulletedList(
        "Parsed 12 files.",
        "Found 1 warning.",
        "Wrote output to ./artifacts."));
```

Pixie nodes are values you can compose, transform, and reuse. This is usually clearer than building one large formatted string.

## Leave Presentation To Renderers

Markup should not contain terminal escape codes or manually inserted line breaks just to fit one terminal width. Use layout nodes such as `WrapBox`, `IndentBox`, and `PrefixBox` when the structure matters.

Renderer tests cover whitespace, wrapping, and degraded output carefully, so behavior that changes the visible shape of terminal output should usually come with rendering tests.

## See It In Context

The [FormattedList example](https://github.com/jonathanvdc/Pixie/blob/master/Examples/FormattedList/Program.cs) shows layout, titles, bullets, colors, decorations, and manual terminal setup.
