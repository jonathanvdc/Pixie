using Pixie.Markup;

namespace Pixie.Transforms
{
    /// <summary>
    /// Minimal visitor base for block-level transforms.
    /// </summary>
    public abstract class MarkupVisitor
    {
        protected abstract bool IsOfInterest(Block node);

        protected abstract Block VisitInteresting(Block node);

        protected virtual Block VisitUninteresting(Block node)
        {
            return node;
        }

        public Block Visit(Block node)
        {
            return IsOfInterest(node)
                ? VisitInteresting(node)
                : VisitUninteresting(node);
        }
    }
}
