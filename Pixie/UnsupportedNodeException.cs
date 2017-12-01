using System;
using System.Runtime.Serialization;

namespace Pixie
{
    /// <summary>
    /// An exception that is thrown when a markup node is encountered that
    /// is not supported directly and does not specify a fallback.
    /// </summary>
    [Serializable]
    public class UnsupportedNodeException : PixieException
    {
        /// <summary>
        /// Creates an unsupported node exception.
        /// </summary>
        public UnsupportedNodeException(MarkupNode node)
            : base("Node not supported.")
        {
            this.Node = node;
        }

        /// <summary>
        /// Creates an unsupported node exception.
        /// </summary>
        /// <param name="node">The node that is not supported.</param>
        /// <param name="message">The exception's error message.</param>
        public UnsupportedNodeException(MarkupNode node, string message)
            : base(message)
        {
            this.Node = node;
        }

        /// <summary>
        /// Creates an unsupported node exception.
        /// </summary>
        /// <param name="node">The node that is not supported.</param>
        /// <param name="message">The exception's error message.</param>
        /// <param name="inner">An inner exception.</param>
        public UnsupportedNodeException(MarkupNode node, string message, Exception inner)
            : base(message, inner)
        {
            this.Node = node;
        }

        /// <summary>
        /// Deserializes an unsupported node exception.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">A streaming context.</param>
        /// <returns></returns>
        protected UnsupportedNodeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Gets the node that triggered this exception.
        /// </summary>
        /// <returns>The unsupported node.</returns>
        public MarkupNode Node { get; private set; }
    }
}