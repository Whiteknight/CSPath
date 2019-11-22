using System.Collections.Generic;
using System.Linq;
using CSPath.Predicates;

namespace CSPath.Paths
{
    /// <summary>
    /// Compares each object to a predicate condition and returns only those objects which satisfy
    /// </summary>
    public class PredicatePath : IPath
    {
        private readonly IReadOnlyList<IPath> _selector;
        private readonly IPathPredicate _predicate;
        private readonly IArityComparer _arity;

        public PredicatePath(IReadOnlyList<IPath> selector, IPathPredicate predicate, IArityComparer arity)
        {
            _selector = selector;
            _predicate = predicate;
            _arity = arity;
        }

        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input) 
            => input.Where(SatisfiesPredicate);

        private bool SatisfiesPredicate(IValueWrapper obj)
        {
            // For each object, apply the selector
            // For each value in the selector result, compare to the value
            // return the object if and only iff all selector values satisfy the comparison
            var values = _selector.Filter(new[] { obj }).Select(v => v.Value).ToList();
            return _arity.IsMatch(values, _predicate.Test);
        }
    }
}
