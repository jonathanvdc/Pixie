# Logging Basics

Pixie's main application-facing abstraction is `ILog`. A log accepts `LogEntry` values, and each entry carries a severity, a title, and a markup tree.

Most applications should acquire one log near startup and pass it to code that needs to report output.

## Choose The Output Stream

Use standard error for diagnostics, warnings, help text, and command-line feedback:

```cs
using Pixie.Terminal;

var log = TerminalLog.Acquire();
```

Use standard output when Pixie is producing normal program output:

```cs
using Pixie.Terminal;

var log = TerminalLog.AcquireStandardOutput();
```

Keeping that distinction matters for shell users. It lets commands pipe data through standard output while keeping diagnostics visible on standard error.

## Log Plain Text

Strings automatically become markup nodes, so the first useful Pixie program can stay small:

```cs
using Pixie;
using Pixie.Terminal;

var log = TerminalLog.Acquire();

log.Info("Reading project file.");
log.Warning("No configuration file was found.");
log.Error("Could not parse command line.");
```

Use the convenience helpers for common severities. When you need full control, create a `LogEntry` directly.

```cs
using Pixie;
using Pixie.Markup;

log.Log(new LogEntry(
    Severity.Info,
    "Build summary",
    new Paragraph("Build completed successfully.")));
```

## Keep Messages Semantic

Logs should describe what the application wants to say. Avoid embedding terminal escape sequences, manual wrapping, or terminal-width assumptions in the message. Put structure in markup nodes and let terminal renderers decide how to display that structure.

## Use Strict Logs In Tests

For tests or command-line tools that treat warnings as failures, `ThrowingLog` can turn selected severities into exceptions. For assertions, `RecordingLog` captures entries so tests can inspect the messages that were logged.
