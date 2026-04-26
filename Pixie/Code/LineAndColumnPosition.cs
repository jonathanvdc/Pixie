namespace Pixie.Code;

/// <summary>
/// Specifies a diagnostic display position in source code.
/// </summary>
/// <remarks>
/// Creates a source position from an identifier, line and column.
/// </remarks>
/// <param name="identifier">The source document identifier.</param>
/// <param name="line">The one-based source line.</param>
/// <param name="column">The one-based source column.</param>
public readonly struct LineAndColumnPosition(string identifier, int line, int column)
{

    /// <summary>
    /// Gets the source document identifier, if known.
    /// </summary>
    /// <returns>The source document identifier.</returns>
    public string Identifier { get; } = identifier;

    /// <summary>
    /// Gets the one-based source line.
    /// </summary>
    /// <returns>The one-based source line.</returns>
    public int Line { get; } = line;

    /// <summary>
    /// Gets the one-based source column.
    /// </summary>
    /// <returns>The one-based source column.</returns>
    public int Column { get; } = column;

    /// <summary>
    /// Gets an unknown source position.
    /// </summary>
    public static LineAndColumnPosition Unknown => default;
}
