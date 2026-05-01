# Option Parsing

Pixie includes GNU-style command-line parsing and generated help output. Define options once, then parse arguments, read typed values, and generate user-facing help from the same definitions.

## Define Options

```cs
using Pixie.Options;

var helpFlag = Option.Flag("-h", "--help");
var filesOption = Option.StringSequence("--files")
    .WithParameter("file");

var commandLine = new CommandLine(
    new Option[] { helpFlag },
    filesOption);
```

## Parse And Read Values

```cs
using System.Collections.Generic;

var parsedArgs = commandLine.Parse(args, log);

bool showHelp = parsedArgs.GetValue(helpFlag);
IReadOnlyList<string> files = parsedArgs.GetValue(filesOption);
```

The option object carries the value type, so callers do not repeat it at the read site.

## Add Help And Version Handling

```cs
var commandLine = new CommandLine(filesOption)
    .WithHelp("Example program.", "example [files-or-options]")
    .WithVersion("example 1.0.0");

var result = commandLine.Parse(args, log);
if (result.WasHandled)
{
    return result.ExitCode;
}
```

`WasHandled` is for parser-managed early exits such as generated help or version output. Parse failures are represented by `IsSuccess == false`.

## Handle Failures

```cs
var result = commandLine.Parse(args, log);

if (!result.IsSuccess || result.WasHandled)
{
    return result.ExitCode;
}
```

Parse problems are reported through `ILog`, which keeps user-facing feedback consistent with the rest of the application.

## See It In Context

- [ParseOptions](https://github.com/jonathanvdc/Pixie/blob/master/Examples/ParseOptions/Program.cs) shows GNU-style parsing, typed values, suggestions, and errors.
- [PrintHelp](https://github.com/jonathanvdc/Pixie/blob/master/Examples/PrintHelp/Program.cs) shows generated help output.
