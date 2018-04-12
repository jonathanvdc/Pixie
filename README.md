# Pixie

[![Travis CI Build Status](https://travis-ci.org/jonathanvdc/Pixie.svg?branch=master)](https://travis-ci.org/jonathanvdc/Pixie)
[![AppVeyor CI Build Status](https://ci.appveyor.com/api/projects/status/twwrupu0k7aaf2x6?svg=true)](https://ci.appveyor.com/project/jonathanvdc/pixie)
[![NuGet](https://img.shields.io/nuget/v/Pixie.svg)](https://www.nuget.org/packages/Pixie)

Pixie is a C# library that prints beautifully formatted output to the console. You describe your layout using a high-level API and Pixie turns it into neatly-formatted text.

Key features:

  * **Caret diagnostics.** Pixie has built-in support for caret diagnostics. Want to point out an error in source code? Pixie's really good at that. It highlights the error and colors both the highlighted text and the squiggle beneath it. Pixie also prints line numbers and even throws in a couple of lines of context.

    ![Diagnostic](docs/img/caret.svg)

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
