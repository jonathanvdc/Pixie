using System;
using System.Runtime.Serialization;

namespace Pixie
{
    /// <summary>
    /// A base class for exceptions specific to Pixie.
    /// </summary>
    [Serializable]
    public class PixieException : Exception
    {
        /// <summary>
        /// Creates a Pixie exception.
        /// </summary>
        public PixieException() { }

        /// <summary>
        /// Creates a Pixie exception.
        /// </summary>
        /// <param name="message">The exception's error message.</param>
        public PixieException(string message) : base(message) { }

        /// <summary>
        /// Creates a Pixie exception.
        /// </summary>
        /// <param name="message">The exception's error message.</param>
        /// <param name="inner">An inner exception.</param>
        public PixieException(string message, Exception inner) : base(message, inner) { }
        
        /// <summary>
        /// Deserializes a Pixie exception.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">A streaming context.</param>
        /// <returns></returns>
        protected PixieException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}