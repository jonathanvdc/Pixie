# Examples

Pixie ships with a small set of focused examples. Each one is meant to show one adoption path clearly rather than cover the entire library.

## SimpleErrorMessage

Project: [`Examples/SimpleErrorMessage`](../Examples/SimpleErrorMessage)

Use this when you want the smallest possible example of a diagnostic-like message with quoted and formatted text.

## FormattedList

Project: [`Examples/FormattedList`](../Examples/FormattedList)

Use this when you want to learn:

- wrapping and layout,
- titles and lists,
- colors and text decoration,
- manual terminal setup with different encodings or style managers.

## PrintHelp

Project: [`Examples/PrintHelp`](../Examples/PrintHelp)

Use this when you want to generate polished help output directly from option definitions.

## ParseOptions

Project: [`Examples/ParseOptions`](../Examples/ParseOptions)

Use this when you want to see:

- GNU-style option parsing,
- typed option values,
- default positional argument handling,
- user-facing parse errors and suggestions.

Try it with:

```sh
dotnet run --project Examples/ParseOptions/ParseOptions.csproj -- --helo file.cs
```

## CaretDiagnostics

Project: [`Examples/CaretDiagnostics`](../Examples/CaretDiagnostics)

Use this when you want:

- source snippets,
- caret highlighting,
- diagnostic headers with file and line information,
- renderer customization for more context lines.

## LoycInterop

Project: [`Examples/LoycInterop`](../Examples/LoycInterop)

Use this when you already use Loyc and want its diagnostics to render through Pixie.

For the package-level setup and rationale, see [loyc.md](loyc.md).
