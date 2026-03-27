# Using Pixie with Loyc

`Pixie.Loyc` is an optional integration package for applications that already depend on Loyc libraries such as EC# or LeMP.

Install it only when you need to translate Loyc diagnostics into Pixie output:

```sh
dotnet add package Pixie.Loyc
```

## What it gives you

The main entry point is `PixieMessageSink`, which implements Loyc's `IMessageSink` and forwards messages into a Pixie `ILog`.

That means Loyc diagnostics can use the same rendering pipeline as the rest of your CLI output, including:

- consistent severities,
- compiler-style headers,
- source snippets,
- wrapping and terminal degradation.

## Minimal setup

```cs
using Pixie;
using Pixie.Loyc;
using Pixie.Terminal;
using Pixie.Transforms;

var log = TerminalLog.Acquire().WithDiagnostics("program");
var messageSink = new PixieMessageSink(log);
```

Once you have a `PixieMessageSink`, pass it anywhere Loyc expects an `IMessageSink`.

## Working example

The repository example at [`Examples/LoycInterop/Program.cs`](../Examples/LoycInterop/Program.cs) shows the full flow:

1. Acquire a Pixie log.
2. Wrap it so entries render as diagnostics.
3. Create a `PixieMessageSink`.
4. Parse Loyc input and let Loyc report issues through that sink.

If you also need source document reuse across multiple inputs, look at `SourceDocumentCache` in the `Pixie.Loyc` package.
