namespace Pixie.Code;

/// <summary>
/// Specifies a diagnostic display position in source code.
/// </summary>
public readonly struct LineAndColumnPosition
{
    /// <summary>
    /// Creates a source position from an identifier, line and column.
    /// </summary>
    /// <param name="identifier">The source document identifier.</param>
    /// <param name="line">The one-based source line.</param>
    /// <param name="column">The one-based source column.</param>
    public LineAndColumnPosition(string identifier, int line, int column)
    {
        this.Identifier = identifier;
        this.Line = line;
        this.Column = column;
    }

    /// <summary>
    /// Gets the source document identifier, if known.
    /// </summary>
    /// <returns>The source document identifier.</returns>
    public string Identifier { get; }

    /// <summary>
    /// Gets the one-based source line.
    /// </summary>
    /// <returns>The one-based source line.</returns>
    public int Line { get; }

    /// <summary>
    /// Gets the one-based source column.
    /// </summary>
    /// <returns>The one-based source column.</returns>
    public int Column { get; }

    /// <summary>
    /// Gets an unknown source position.
    /// </summary>
    public static LineAndColumnPosition Unknown => default;
}
