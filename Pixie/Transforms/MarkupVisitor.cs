using Pixie.Markup;

namespace Pixie.Transforms
{
    /// <summary>
    /// Minimal visitor base for block-level transforms.
    /// </summary>
    public abstract class MarkupVisitor
    {
        /// <summary>
        /// Determines whether a block should be handled by <see cref="VisitInteresting(Block)"/>.
        /// </summary>
        /// <param name="node">The block to inspect.</param>
        /// <returns><c>true</c> if the block is of interest; otherwise, <c>false</c>.</returns>
        protected abstract bool IsOfInterest(Block node);

        /// <summary>
        /// Visits a block that is considered interesting.
        /// </summary>
        /// <param name="node">The block to visit.</param>
        /// <returns>The transformed block.</returns>
        protected abstract Block VisitInteresting(Block node);

        /// <summary>
        /// Visits a block that is not considered interesting.
        /// </summary>
        /// <param name="node">The block to visit.</param>
        /// <returns>The transformed block.</returns>
        protected virtual Block VisitUninteresting(Block node)
        {
            return node;
        }

        /// <summary>
        /// Visits a block.
        /// </summary>
        /// <param name="node">The block to visit.</param>
        /// <returns>The transformed block.</returns>
        public Block Visit(Block node)
        {
            return IsOfInterest(node)
                ? VisitInteresting(node)
                : VisitUninteresting(node);
        }
    }
}
