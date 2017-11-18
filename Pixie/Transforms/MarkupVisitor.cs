namespace Pixie.Transforms
{
    /// <summary>
    /// A base class for markup node visitors.
    /// </summary>
    public abstract class MarkupVisitor
    {
        /// <summary>
        /// Tells if a node is of interest to this visitor.
        /// Visitors can specify unique behavior for interesting
        /// nodes, whereas uninteresting nodes are always treated
        /// the same: the visitor simply visits their children.
        /// </summary>
        /// <param name="node">A markup node.</param>
        /// <returns>
        /// <c>true</c> if the node is interesting; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsOfInterest(MarkupNode node);

        /// <summary>
        /// Visits a node that has been marked as interesting.
        /// </summary>
        /// <param name="node">A node to visit.</param>
        /// <returns>A visited node.</returns>
        protected abstract MarkupNode VisitInteresting(MarkupNode node);

        /// <summary>
        /// Visits a node that has not been marked as interesting.
        /// </summary>
        /// <param name="node">A node to visit.</param>
        /// <returns>A visited node.</returns>
        protected MarkupNode VisitUninteresting(MarkupNode node)
        {
            return node.Map(Visit);
        }

        /// <summary>
        /// Visits a markup node.
        /// </summary>
        /// <param name="node">A node to visit.</param>
        /// <returns>A visited node.</returns>
        public MarkupNode Visit(MarkupNode node)
        {
            if (IsOfInterest(node))
            {
                return VisitInteresting(node);
            }
            else
            {
                return VisitUninteresting(node);
            }
        }
    }
}