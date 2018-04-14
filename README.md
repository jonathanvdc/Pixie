# Pixie

[![Travis CI Build Status](https://travis-ci.org/jonathanvdc/Pixie.svg?branch=master)](https://travis-ci.org/jonathanvdc/Pixie)
[![AppVeyor CI Build Status](https://ci.appveyor.com/api/projects/status/twwrupu0k7aaf2x6?svg=true)](https://ci.appveyor.com/project/jonathanvdc/pixie)
[![NuGet](https://img.shields.io/nuget/v/Pixie.svg)](https://www.nuget.org/packages/Pixie)

Pixie is a C# library that prints beautifully formatted output to the console. You describe your layout using a high-level API and Pixie turns it into neatly-formatted text.

Essentially, Pixie is all about creating gorgeous console applications with minimal effort.

Key features:

  * **Caret diagnostics.** Pixie has built-in support for caret diagnostics. Want to point out an error in source code? Pixie's really good at that. It highlights the error and colors both the highlighted text and the squiggle beneath it. Pixie also prints line numbers and even throws in a couple of lines of context.

    ![Diagnostic](docs/img/caret.svg)

  * **Argument parsing.** In addition to formatting application output, Pixie can also parse command-line arguments.

    When parsing command-line arguments, Pixie tries to go the extra mile and guide users toward correct program usage by spellchecking options and proactively printing option usage. Here's what that looks like in action.

    ![Option parsing feedback](docs/img/option-parsing-feedback.svg)

  * **Help messages.** Help messages are wonderful for users but they're kind of tedious to write and maintain. Pixie generates them automatically from the same data that is used to parse command-line arguments.
  
    Here's an excerpt from an example help message. The formatting, grouping and fancy Unicode characters are all automagically generated by Pixie.

    ![Help message](docs/img/help-message.svg)

  * **Graceful degradation.** Pixie tries to make its output as pretty as your terminal will allow and degrades its output gracefully on terminal implementations that don't support all of Unicode.
  
    For example, when rendered on `xterm` (a Unix terminal), this bulleted list item uses Unicode characters and ANSI control sequences:

    ![Fancy bullets](docs/img/degradation-fancy.svg)

    The default Windows console doesn't support those features, so Pixie uses ASCII characters and the `System.Console` API there:

    ![Simple bullets](docs/img/degradation-simple.svg)

  * **Pretty output.** Pixie makes a real effort to produce good-looking output. It supports aligning and word-wrapping text. It also has built-in support for common types of messages, like diagnostics (errors, warnings, etc.) and help messages.

    To see why this is something you might want, just take a look at the formatting of `mcs`'s error message below. Note in particular how the word "suitable" is split up awkwardly.

    ![Sad line break](docs/img/sad-line-break.svg)

    Here's what that error message would have looked like if `mcs` used Pixie. Much better, right?

    ![Happy line break](docs/img/happy-line-break.svg)

  * **Loyc interop.** Pixie can gracefully translate and log diagnostics produced by [Loyc](https://github.com/qwertie/ecsharp) libraries (including the EC# parser and LeMP). The optional Loyc interop logic is bundled in the `Pixie.Loyc` package, which can be installed in addition to the regular `Pixie` package.

    Here's what a diagnostic produced by Loyc looks like after Pixie has translated it:

    ![Loyc diagnostic](docs/img/loyc-interop.svg)

  * **Customization.** Pixie is customizable: you can easily configure the existing renderers and define your own markup elements and renderers.

## Getting started

To use Pixie in one of your projects, simply install the [Pixie NuGet package](https://www.nuget.org/packages/Pixie) and start coding. Here are some basic first steps:

  * **Acquiring a log.** Your application will probably want to send output to the terminal. Pixie's preferred abstraction for doing that is the `ILog`. All logs define a `void Log(LogEntry)` method, which sends a single, self-contained message to the log.

    There are many log implementations, but you probably want a log that sends messages straight to the terminal. You can acquire one of those like so:

    ```cs
    using Pixie.Terminal;
    // ...
    var log = TerminalLog.Acquire();
    ```

    > **Pro tip:** acquiring a log is something you want to do only once, for concurrency and performance reasons.

  * **Logging messages.** As stated before, a `LogEntry` is a self-contained message that can be sent to any log. It consists of a `Severity` (which can be either `Error`, `Warning`, `Message` or `Info`) and a `MarkupNode`. The latter is essentially a description of what you want the user to see.

    A `MarkupNode` implementation can be anything ranging from a lowly text message to a full-blown caret diagnostic. In this example, we'll just log a text message. Those can be created by calling `new Text("message")` or by implicitly converting a `string` to a `MarkupNode`.

    Putting it all together:

    ```cs
    using Pixie;
    using Pixie.Markup;
    // ...
    log.Log(
        new LogEntry(
            Severity.Error,
            new Text("Something went horribly wrong.")));
    ```

  * **Parsing command-line arguments.** Pixie's approach to parsing command-line arguments is pretty standard. You define a set of options and then use those to parse command-line arguments.

    ```cs
    using Pixie.Options;
    // ...
    // Define a --help/-h flag option.
    var helpFlag = FlagOption.CreateFlagOption(
        OptionForm.Short("h"),
        OptionForm.Long("help"));

    // Define a pseudo-option for positional arguments.
    var filesOption = SequenceOption.CreateStringOption(
        OptionForm.Long("files"));

    // Create a parser for GNU-style command-line arguments.
    var parser = new GnuOptionSetParser(
        new Option[] { helpFlag }, // <-- options
        filesOption); // <-- pseudo-option for positional arguments

    // Parse command-line arguments. The parser automatically
    // recovers from errors and sends error messages to `log`.
    var parsedArgs = parser.Parse(new[] { "hi", "-h" }, log);

    // Recover parsed arguments.
    bool showHelp = parsedArgs.GetValue<bool>(helpFlag);
    string[] files = parsedArgs.GetValue<string[]>(filesOption);
    ```

  * **Documenting options.** Adding documentation to your options is easy. Just call `WithCategory`, `WithDescription` and/or `WithParameters` on the options you'd like to document.

    For example, here's how to document the `-x` option from `gcc` and `clang`.

    ```cs
    xOption = SequenceOption.CreateStringOption(OptionForm.Short("x"))
        .WithCategory("Input and output")
        .WithDescription(
          "Specify explicitly the language for the " +
          "following input files.")
        .WithParameters(
            new SymbolicOptionParameter("language"), // <-- not varargs
            new SymbolicOptionParameter("file", true)); // <-- varargs
    ```

## Contributing

Thank you so much for considering to contribute! Feel free to fork the Pixie repository and send me a pull request&mdash;I'd love to merge it in! It'd also be nice if you opened an issue beforehand so we can talk about your wonderful new feature.

Alternatively, if you think you found a bug or just have a question about Pixie: open an issue.
