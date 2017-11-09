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
        public PixieException() { }
        public PixieException(string message) : base(message) { }
        public PixieException(string message, Exception inner) : base(message, inner) { }
        protected PixieException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}