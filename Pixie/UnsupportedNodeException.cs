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
        public UnsupportedNodeException(MarkupNode node)
            : base("Node not supported.")
        {
            this.Node = node;
        }

        public UnsupportedNodeException(MarkupNode node, string message)
            : base(message)
        {
            this.Node = node;
        }

        public UnsupportedNodeException(MarkupNode node, string message, Exception inner)
            : base(message, inner)
        {
            this.Node = node;
        }

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