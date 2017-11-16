# Pixie

Pixie is a C# library that prints beautifully formatted output to the console. You describe your layout using a high-level API and Pixie turns it into neatly-formatted text.

Reasons to use Pixie:

  * **Pixie aligns your text and inserts line breaks when the screen is full.** It also supports word-wrapping.

    If you use word-wrapping, then you can rest assured that things like this unfortunate line break won't happen.

    ```
    error CS5001: Program `Color.exe' does not contain a static `Main' method suitab
    le for an entry point
    ```

  * **Pixie tries to make its output as pretty as your terminal will allow** and degrades its output gracefully on terminal implementations that don't support all of Unicode.
  
    For example, when rendered on `xterm` (a Unix terminal), this bulleted list item uses Unicode bullets and quotes:

    ```
     •  “Lorem ipsum dolor sit amet, consectetur adipiscing elit. ...”
    ```

    The default Windows console doesn't support those features, so Pixie uses ASCII characters there:

    ```
     *  "Lorem ipsum dolor sit amet, consectetur adipiscing elit. ..."
    ```

  * **Pixie has built-in support for caret diagnostics.** Want to point out an error in source code? Pixie's really good at that. It highlights the error and colors both the highlighted text and the squiggle beneath it. Pixie also prints line numbers and even throws in a couple of lines of context.

    Caret diagnostics look better when viewed in the terminal, but the example below should give you an idea of what Pixie's caret diagnostics look like.

    ```
    1 │ public static class Program
    2 │ {
    3 │     public Program()
      │     ~~~~~~ ^~~~~~~~~
    4 │     { }
    5 │ }
    ```

  * **Pixie is customizable:** you can easily configure the existing renderers and define your own markup elements and renderers.
