using System;

namespace Pixie.Options
{
    /// <summary>
    /// Defines a form for an option.
    /// </summary>
    public struct OptionForm : IEquatable<OptionForm>
    {
        /// <summary>
        /// Creates an option form.
        /// </summary>
        /// <param name="name">
        /// The option form's name.
        /// </param>
        /// <param name="isShort">
        /// Tells if the option form is a short form.
        /// </param>
        public OptionForm(string name, bool isShort)
        {
            this = default(OptionForm);
            this.Name = name;
            this.IsShort = isShort;
        }

        /// <summary>
        /// Gets the name of this option form. This does not
        /// include any prefixes.
        /// </summary>
        /// <returns>The name of the option form.</returns>
        public string Name { get; private set; }

        /// <summary>
        /// Tells if this option form is a short form. Short
        /// forms may be treated differently by the option
        /// parsed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the option form is a short form; otherwise, <c>false</c>.
        /// </returns>
        public bool IsShort { get; private set; }

        /// <summary>
        /// Checks if this option form equals another.
        /// </summary>
        /// <param name="other">The other option form.</param>
        /// <returns><c>true</c> if the option forms are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(OptionForm other)
        {
            return Name == other.Name
                && IsShort == other.IsShort;
        }

        /// <summary>
        /// Checks if two option forms are the same.
        /// </summary>
        /// <param name="first">The first form to compare.</param>
        /// <param name="second">The second form to compare.</param>
        /// <returns>
        /// <c>true</c> if the first form equals the second; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator==(OptionForm first, OptionForm second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Checks if two option forms are different.
        /// </summary>
        /// <param name="first">The first form to compare.</param>
        /// <param name="second">The second form to compare.</param>
        /// <returns>
        /// <c>true</c> if the first form does not equal the second; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator!=(OptionForm first, OptionForm second)
        {
            return !first.Equals(second);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return (IsShort ? "-" : "--") + Name;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (Name.GetHashCode() << 1) ^ IsShort.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is OptionForm && Equals((OptionForm)obj);
        }
    }
}