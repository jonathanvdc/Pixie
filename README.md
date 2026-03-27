# Pixie

[![CI](https://github.com/jonathanvdc/Pixie/actions/workflows/ci.yml/badge.svg)](https://github.com/jonathanvdc/Pixie/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Pixie.svg)](https://www.nuget.org/packages/Pixie)

Pixie is a C# library for building polished command-line applications. It gives you a high-level API for terminal output, diagnostics, help text, and GNU-style option parsing, then renders the best output your terminal can support.

Pixie is a good fit when you want to:

* print structured, readable console output instead of hand-formatting strings,
* show rich diagnostics with source context and caret highlights,
* generate help output from the same option definitions you parse, and
* keep CLI output readable across terminals with different Unicode and styling support.

## Quick start

Install Pixie:

```sh
dotnet add package Pixie
```

Then write and log a message:

```cs
using Pixie;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

log.Info("Hello from Pixie.");
```

`TerminalLog.Acquire()` is the usual entry point for diagnostics and other CLI feedback. It writes to standard error by default, which is often what you want for warnings, errors, and help output.

If you want to write normal program output to standard output instead, use:

```cs
using Pixie.Terminal;

var log = TerminalLog.AcquireStandardOutput();
```

In most applications, you should acquire a log once and reuse it.

## Packages

Pixie is split into a small set of packages and assemblies:

| Package | Purpose |
| --- | --- |
| [`Pixie`](https://www.nuget.org/packages/Pixie) | Core logging, markup, diagnostics, option parsing, and terminal integration. |
| `Pixie.Terminal` | Terminal rendering types. This assembly ships with the `Pixie` package, so you usually do not install it separately. |
| [`Pixie.Loyc`](https://www.nuget.org/packages/Pixie.Loyc) | Optional Loyc interoperability for translating Loyc diagnostics into Pixie output. |

Install Loyc support only if you need it:

```sh
dotnet add package Pixie.Loyc
```

## Choose the right starting point

Use this as a quick decision guide:

| If you want to... | Start with... |
| --- | --- |
| Write regular terminal output with wrapping and layout | `TerminalLog.AcquireStandardOutput()` |
| Write diagnostics, warnings, or help text | `TerminalLog.Acquire()` |
| Turn ordinary log entries into compiler-style diagnostics | `log.WithDiagnostics("program")` |
| Parse GNU-style options and read typed values back | `GnuOptionSetParser` + `OptionSet` |
| Generate `--help` output from option definitions | `HelpMessage` |
| Control styling, encoding, or terminal capabilities manually | `TextWriterTerminal` + `TerminalLog.Acquire(...)` |
| Translate Loyc diagnostics into Pixie output | `PixieMessageSink` from `Pixie.Loyc` |

For a fuller walkthrough, see [docs/getting-started.md](docs/getting-started.md).

## What Pixie can do

### Caret diagnostics

Pixie has built-in support for compiler-style diagnostics. In Pixie, a diagnostic is a message with:

* an origin, such as a file location or application name,
* a kind, such as `error` or `warning`,
* a short title, and
* an optional body with extra context, such as highlighted source code.

That means Pixie can render both the diagnostic header, like `code.cs:3:5: error: expected token`, and the caret-highlighted snippet underneath it.

![Diagnostic](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/caret.svg)

### Argument parsing with feedback

Pixie can parse GNU-style command-line options and report mistakes in a user-friendly way, including usage guidance and option name suggestions.

![Option parsing feedback](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/option-parsing-feedback.svg)

### Help messages

Pixie can generate help output from the same option definitions you use for parsing, so parsing and documentation stay in sync.

![Help message](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/help-message.svg)

### Graceful terminal degradation

Pixie tries to produce the nicest output your terminal can handle. When Unicode characters or ANSI styling are unavailable, it falls back to simpler representations instead of breaking formatting.

Unicode-rich rendering:

![Fancy bullets](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/degradation-fancy.svg)

Simpler fallback rendering:

![Simple bullets](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/degradation-simple.svg)

### Readable layout

Pixie supports alignment, wrapping, and reusable markup nodes for common CLI output patterns.

Without careful layout, terminal messages can become awkward:

![Sad line break](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/sad-line-break.svg)

With Pixie, the same message can be wrapped and structured more cleanly:

![Happy line break](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/happy-line-break.svg)

### Loyc interoperability

If you use Loyc libraries such as EC# or LeMP, `Pixie.Loyc` can translate their diagnostics into Pixie markup so they render consistently with the rest of your application output.

![Loyc diagnostic](https://raw.githubusercontent.com/jonathanvdc/Pixie/master/docs/img/loyc-interop.svg)

## Getting started

### 1. Acquire a log

Pixie's main output abstraction is `ILog`. Logs accept `LogEntry` values, which are self-contained messages with a severity and a markup tree.

For terminal applications, `TerminalLog.Acquire()` is the usual choice when you want output on standard error:

```cs
using Pixie.Terminal;

var log = TerminalLog.Acquire();
```

If you want standard output instead, use `TerminalLog.AcquireStandardOutput()`.

### 2. Log a message

A `LogEntry` consists of a `Severity` and a `MarkupNode`. Plain strings can be used directly as markup, so the smallest useful example is:

```cs
using Pixie;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

log.Error("Something went wrong.");
```

If you want explicit markup nodes, use types from `Pixie.Markup`, such as `Text`, `Sequence`, `BulletedList`, or `HighlightedSource`.

### 3. Parse command-line arguments

Pixie includes GNU-style option parsing. You define options once, then parse arguments and read back typed values.

```cs
using Pixie;
using Pixie.Options;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

var helpFlag = Option.Flag("-h", "--help");
var filesOption = Option.StringSequence("--files")
    .WithParameter("file");

var commandLine = new CommandLine(
    new Option[] { helpFlag },
    filesOption);

var parsedArgs = commandLine.Parse(new[] { "input.cs", "-h" }, log);

bool showHelp = parsedArgs.GetValue<bool>(helpFlag);
string[] files = parsedArgs.GetValue<string[]>(filesOption);

if (!parsedArgs.IsSuccess)
{
    return;
}
```

If you want the common `--help` and `--version` flow handled for you:

```cs
var commandLine = new CommandLine(filesOption)
    .WithHelp("Example program.", "example [files-or-options]")
    .WithVersion("example 1.0.0");

var result = commandLine.Parse(args, log);
if (result.WasHandled)
{
    return;
}
```

For custom typed parsing, use the generic builders:

```cs
var portOption = Option.Value(
    (OptionForm form, string argument, ILog log) => int.Parse(argument),
    8080,
    "--port")
    .WithParameter("n");
```

When parsing fails, Pixie can report errors to the log and also capture them in the returned `OptionParseResult`. The same result can also tell you whether built-in help or version handling already printed output via `WasHandled`. In other words, `WasHandled` is for parser-managed early exits like help/version, not for parse failures.

By design, `Parse(...)` still returns an `OptionSet`, so applications can decide how to recover. A common pattern is to capture log entries and then bail out if any errors were reported:

```cs
using Pixie;
using Pixie.Options;
using Pixie.Terminal;

var terminalLog = TerminalLog.Acquire();
var recordingLog = new RecordingLog(terminalLog);

var parser = new GnuOptionSetParser(new Option[] { helpFlag }, filesOption);
var parsedArgs = parser.Parse(args, recordingLog);

if (recordingLog.Contains(Severity.Error))
{
    return 1;
}
```

If you prefer exceptions in tests or strict tooling, `TestLog` can turn selected severities into failures.

### 4. Document options and generate help

Options can carry categories, descriptions, and parameter metadata. That same information can then be used to generate help output.

```cs
using Pixie.Markup;
using Pixie.Options;

var xOption = SequenceOption.CreateStringOption(OptionForm.Short("x"))
    .WithCategory("Input and output")
    .WithDescription(
        "Specify explicitly the language for the following input files.")
    .WithParameters(
        new SymbolicOptionParameter("language"),
        new SymbolicOptionParameter("file", true));

var help = new HelpMessage(
    "An example application built with Pixie.",
    "example [files-or-options]",
    new Option[] { xOption });
```

To see a more complete example, check [Examples/PrintHelp/Program.cs](Examples/PrintHelp/Program.cs).

### 5. Render rich diagnostics

Pixie's diagnostic model is especially useful when you already know the source span you want to highlight.

There are two layers here:

* `HighlightedSource` renders a code snippet with line numbers and caret/squiggle markers.
* `WithDiagnostics(...)` wraps log entries as full diagnostics so they also get a header like `code.cs:3:5: error: Expected constructor name`.

In practice, most applications want both, so the usual setup is to enable diagnostics once on the log and then log `HighlightedSource` nodes normally.

```cs
using Pixie;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Terminal;
using Pixie.Transforms;

var log = TerminalLog.Acquire().WithDiagnostics("program");

const string source = "public static class Program\n{\n    public Program()\n    { }\n}";
var document = new StringDocument("code.cs", source);
var nameOffset = source.IndexOf("Program()");

var focusRegion = new SourceRegion(
    new SourceSpan(document, nameOffset, "Program".Length));

log.Log(new LogEntry(
    Severity.Error,
    "Expected constructor name",
    new HighlightedSource(focusRegion, focusRegion)));
```

This works because `log.WithDiagnostics("program")` converts each `LogEntry` into a diagnostic before rendering it. When the entry contains a `HighlightedSource`, Pixie uses that source span as the diagnostic origin, which is what makes the filename and line/column information appear in the header.

If you log a raw `LogEntry` with `new HighlightedSource(...)` but do not wrap the log with `WithDiagnostics(...)`, Pixie will still render the snippet and caret, but it will not render the `code.cs:line:column: error: ...` header.

For a fuller version with transforms and custom renderer configuration, see [Examples/CaretDiagnostics/Program.cs](Examples/CaretDiagnostics/Program.cs).

## Package notes

`Pixie` is enough for most applications. It includes the core APIs plus the `Pixie.Terminal` assembly, so you can use `TerminalLog`, terminal renderers, and terminal device helpers after installing the main package.

`Pixie.Loyc` is optional. Install it only when you already use Loyc libraries and want their diagnostics to flow through Pixie. For a dedicated walkthrough, see [docs/loyc.md](docs/loyc.md).

## Examples

The repository includes small example programs you can run directly:

```sh
dotnet run --project Examples/PrintHelp/PrintHelp.csproj
dotnet run --project Examples/ParseOptions/ParseOptions.csproj -- --helo file.cs
dotnet run --project Examples/CaretDiagnostics/CaretDiagnostics.csproj
```

Available examples:

| Example | What it demonstrates |
| --- | --- |
| [`Examples/SimpleErrorMessage`](Examples/SimpleErrorMessage) | A minimal diagnostic-style message. |
| [`Examples/FormattedList`](Examples/FormattedList) | Titles, colors, wrapping, bullets, and manual terminal configuration. |
| [`Examples/PrintHelp`](Examples/PrintHelp) | Building help output from option definitions. |
| [`Examples/ParseOptions`](Examples/ParseOptions) | GNU-style parsing, typed values, suggestions, and diagnostic feedback. |
| [`Examples/CaretDiagnostics`](Examples/CaretDiagnostics) | Source snippets, focus regions, and compiler-style diagnostic headers. |
| [`Examples/LoycInterop`](Examples/LoycInterop) | Translating Loyc parser diagnostics into Pixie output. |

For a more guided tour, see [docs/examples.md](docs/examples.md).

## Building and testing

Build the solution:

```sh
dotnet build Pixie.sln
```

Run the test suite:

```sh
dotnet test Tests/Tests.csproj
```

## Repository layout

| Path | Contents |
| --- | --- |
| [`Pixie/`](Pixie) | Core library: logs, markup nodes, diagnostics, options, and transforms. |
| [`Pixie.Terminal/`](Pixie.Terminal) | Terminal rendering and device-specific behavior. |
| [`Pixie.Loyc/`](Pixie.Loyc) | Loyc interoperability layer. |
| [`Examples/`](Examples) | Small runnable sample applications. |
| [`Tests/`](Tests) | NUnit test suite. |
| [`docs/img/`](docs/img) | SVG assets used in this README. |

## Customization

Pixie is designed to be extensible. You can:

* compose markup trees from reusable node types,
* configure or replace renderers,
* wrap logs in transforms, and
* define your own markup elements and rendering behavior.

The examples in [Examples/CaretDiagnostics/Program.cs](Examples/CaretDiagnostics/Program.cs) and [Examples/ParseOptions/Program.cs](Examples/ParseOptions/Program.cs) show some of that flexibility in practice.

## Contributing

Issues, questions, and pull requests are all welcome.

If you want to contribute code:

1. Open an issue first for larger changes so the direction is clear.
2. Build the solution with `dotnet build Pixie.sln`.
3. Run tests with `dotnet test Tests/Tests.csproj`.

Bug reports and usage questions are also welcome in the issue tracker.
