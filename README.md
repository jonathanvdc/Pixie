# Pixie

[![CI](https://github.com/jonathanvdc/Pixie/actions/workflows/ci.yml/badge.svg)](https://github.com/jonathanvdc/Pixie/actions/workflows/ci.yml)
[![Docs](https://github.com/jonathanvdc/Pixie/actions/workflows/docs.yml/badge.svg)](https://github.com/jonathanvdc/Pixie/actions/workflows/docs.yml)
[![NuGet](https://img.shields.io/nuget/v/Pixie.svg)](https://www.nuget.org/packages/Pixie)

Pixie is a C# library for building polished command-line applications. It gives you semantic building blocks for terminal output, diagnostics, help text, and GNU-style option parsing, then renders the best output your terminal can support.

## Install

```sh
dotnet add package Pixie
```

The `Pixie` package includes both the core APIs and the `Pixie.Terminal` assembly.

## Quick Start

```cs
using Pixie;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

log.Info("Hello from Pixie.");
```

`TerminalLog.Acquire()` writes to standard error, which is usually right for diagnostics, warnings, help output, and other command-line feedback. Use `TerminalLog.AcquireStandardOutput()` when Pixie is producing normal program output.

## Documentation

The full documentation site is published with GitHub Pages:

- [Documentation](https://jonathanvdc.github.io/Pixie/)
- [Getting started](https://jonathanvdc.github.io/Pixie/getting-started.html)
- [API reference](https://jonathanvdc.github.io/Pixie/api/)

The source for the documentation lives under [`docs/`](docs), and generated API reference is built from the public C# surface with DocFX.

## What Pixie Is For

| If you want to... | Start with... |
| --- | --- |
| Write regular terminal output with wrapping and layout | `TerminalLog.AcquireStandardOutput()` |
| Write diagnostics, warnings, or help text | `TerminalLog.Acquire()` |
| Write compiler-style diagnostics with headers | `log.ErrorDiagnostic(...)` or `Diagnostic.FromSeverity(...)` |
| Parse GNU-style options and read typed values back | `CommandLine` + `OptionParseResult` |
| Generate `--help` output from option definitions | `CommandLine.WithHelp(...)` or `HelpMessage` |
| Control styling, encoding, or terminal capabilities manually | `TextWriterTerminal` + `TerminalLog.Acquire(...)` |

## Examples

The repository includes focused example programs:

```sh
dotnet run --project Examples/PrintHelp/PrintHelp.csproj
dotnet run --project Examples/ParseOptions/ParseOptions.csproj -- --helo file.cs
dotnet run --project Examples/CaretDiagnostics/CaretDiagnostics.csproj
```

| Example | What it demonstrates |
| --- | --- |
| [`Examples/SimpleErrorMessage`](Examples/SimpleErrorMessage) | A minimal diagnostic-style message. |
| [`Examples/FormattedList`](Examples/FormattedList) | Titles, colors, wrapping, bullets, and manual terminal setup. |
| [`Examples/PrintHelp`](Examples/PrintHelp) | Help output from option definitions. |
| [`Examples/ParseOptions`](Examples/ParseOptions) | GNU-style parsing, typed values, suggestions, and parse feedback. |
| [`Examples/CaretDiagnostics`](Examples/CaretDiagnostics) | Source snippets, focus regions, and diagnostic headers. |

## Build And Test

```sh
dotnet build Pixie.slnx
dotnet test Pixie.slnx
```

Build the documentation locally with DocFX:

```sh
dotnet tool install --global docfx --version 2.78.5
docfx docfx.json
```

The generated site is written to `_site/`.

## Contributing

Issues, questions, and pull requests are welcome. For larger changes, open an issue first so the direction is clear.
