# Getting Started with Pixie

Pixie is easiest to adopt when you treat it as three layers:

1. `ILog` and `LogEntry` for application-facing output.
2. Markup nodes for structured terminal content.
3. Optional helpers for diagnostics, option parsing, and help generation.

## Pick the right log

For diagnostics and CLI feedback, start with standard error:

```cs
using Pixie.Terminal;

var log = TerminalLog.Acquire();
```

For normal program output, prefer standard output:

```cs
using Pixie.Terminal;

var log = TerminalLog.AcquireStandardOutput();
```

In both cases, acquire a log once and reuse it throughout the application.

## Log plain messages first

The smallest useful Pixie program logs a `LogEntry` with a severity and some markup:

```cs
using Pixie;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

log.Log(new LogEntry(
    Severity.Info,
    "Hello from Pixie."));
```

Strings automatically become markup nodes, so you can start simple and add structure later.

## Add structure with markup nodes

When plain strings stop being enough, compose output with types from `Pixie.Markup`.

Common building blocks:

| Type | Use it for |
| --- | --- |
| `Text` | Explicit text nodes. |
| `Sequence` | Concatenating multiple markup nodes. |
| `BulletedList` | Lists with automatic bullets and spacing. |
| `WrapBox` | Word- or character-wrapped content. |
| `Title` | Section headings. |
| `ColorSpan` and `DecorationSpan` | Styling. |
| `HighlightedSource` | Source snippets with highlighted regions. |

The [FormattedList example](../Examples/FormattedList/Program.cs) is a good reference for layout and wrapping.

## Turn entries into diagnostics

If you want compiler-style output such as `file.cs:12:4: error: expected expression`, wrap your log once:

```cs
using Pixie.Terminal;
using Pixie.Transforms;

var log = TerminalLog.Acquire().WithDiagnostics("program");
```

After that, ordinary `LogEntry` values are rendered as diagnostics automatically. When a log entry contains a `HighlightedSource`, Pixie can use its source span to populate the filename and line/column header.

See the [CaretDiagnostics example](../Examples/CaretDiagnostics/Program.cs) for the full pattern.

## Parse options and keep docs in sync

Pixie's option parser and help output are designed to share the same definitions:

```cs
using Pixie.Options;

var helpFlag = FlagOption.CreateFlagOption(
    OptionForm.Short("h"),
    OptionForm.Long("help"));

var filesOption = SequenceOption.CreateStringOption(
    OptionForm.Long("files"));

var parser = new GnuOptionSetParser(
    new Option[] { helpFlag },
    filesOption);
```

Once parsed, read typed values back through `OptionSet`:

```cs
bool showHelp = parsedArgs.GetValue<bool>(helpFlag);
string[] files = parsedArgs.GetValue<string[]>(filesOption);
```

The same option definitions can feed `HelpMessage`, which keeps parsing and `--help` output aligned.

See:

- [PrintHelp](../Examples/PrintHelp/Program.cs) for help generation.
- [ParseOptions](../Examples/ParseOptions/Program.cs) for parsing and user-facing error reporting.

## Decide how to handle parse failures

`GnuOptionSetParser.Parse(...)` logs problems but still returns an `OptionSet`. That gives applications room to recover, continue, or abort as needed.

For command-line applications, a practical pattern is:

```cs
using Pixie;
using Pixie.Options;
using Pixie.Terminal;

var terminalLog = TerminalLog.Acquire();
var recordingLog = new RecordingLog(terminalLog);

var parsedArgs = parser.Parse(args, recordingLog);

if (recordingLog.Contains(Severity.Error))
{
    return 1;
}
```

For tests or strict tooling, `TestLog` can throw when selected severities are logged.

## Customize rendering only when needed

The default terminal acquisition methods are usually enough. Reach for lower-level terminal APIs when you need to control:

- output width,
- encoding,
- ANSI styling,
- console styling,
- degraded rendering for limited terminals.

The [FormattedList example](../Examples/FormattedList/Program.cs) shows how to build a custom `TextWriterTerminal` and pass it into `TerminalLog.Acquire(...)`.
