using Pixie.Markup;

namespace Pixie.Options
{
    /// <summary>
    /// A base class for option parameter definitions. Option
    /// parameters are intended for use in option documentation.
    /// Their intent is to communicate how to use an option,
    /// not to formally define how arguments are parsed.
    /// </summary>
    public abstract class OptionParameter
    {
        /// <summary>
        /// Represents the parameter as a markup node.
        /// </summary>
        /// <returns>A markup node.</returns>
        public abstract MarkupNode Representation { get; }
    }

    /// <summary>
    /// An option parameter that is a stand-in for some argument.
    /// </summary>
    public sealed class SymbolicOptionParameter : OptionParameter
    {
        /// <summary>
        /// Creates a symbolic option parameter.
        /// </summary>
        /// <param name="name">The parameter's name.</param>
        public SymbolicOptionParameter(string name)
            : this(name, false)
        { }

        /// <summary>
        /// Creates a symbolic option parameter.
        /// </summary>
        /// <param name="name">The parameter's name.</param>
        /// <param name="isVarargs">
        /// Tells if the parameter can take more than one argument.
        /// </param>
        public SymbolicOptionParameter(string name, bool isVarargs)
        {
            this.Name = name;
            this.IsVarargs = isVarargs;
        }

        /// <summary>
        /// Gets the parameter's name.
        /// </summary>
        /// <returns>The name.</returns>
        public string Name { get; private set; }

        /// <summary>
        /// Tells if the parameter can take more than one argument.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the parameter can take more than one argument; otherwise, <c>false</c>.
        /// </returns>
        public bool IsVarargs { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Representation
        {
            get
            {
                return new MathSymbol(
                    IsVarargs
                    ? Name + "..."
                    : Name);
            }
        }
    }

    /// <summary>
    /// An option parameter that always takes the same value.
    /// </summary>
    public sealed class VerbatimOptionParameter : OptionParameter
    {
        /// <summary>
        /// Creates a verbatim option parameter.
        /// </summary>
        /// <param name="contents">
        /// The markup node that represents the parameter.
        /// </param>
        public VerbatimOptionParameter(MarkupNode contents)
        {
            this.Contents = contents;
        }

        /// <summary>
        /// Gets the markup node that is rendered verbatim
        /// as a parameter.
        /// </summary>
        /// <returns>The parameter.</returns>
        public MarkupNode Contents { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Representation => Contents;
    }

    /// <summary>
    /// An option parameter that is optional: it may or may not be present.
    /// </summary>
    public sealed class OptionalOptionParameter : OptionParameter
    {
        /// <summary>
        /// Creates an optional parameter.
        /// </summary>
        /// <param name="parameter">
        /// A parameter that may or may not be present.
        /// </param>
        public OptionalOptionParameter(OptionParameter parameter)
        {
            this.Parameter = parameter;
        }

        /// <summary>
        /// Gets the inner parameter that is optional.
        /// </summary>
        /// <returns></returns>
        public OptionParameter Parameter { get; private set; }

        /// <inheritdoc/>
        public override MarkupNode Representation =>
            new Sequence(
                "[",
                Parameter.Representation,
                "]");
    }
}
