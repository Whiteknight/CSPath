using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    /// <summary>
    /// Compares each object to a predicate condition and returns only those objects which satisfy
    /// </summary>
    public class PredicatePath : IPath
    {
        private readonly IReadOnlyList<IPath> _selector;
        private readonly object _value;
        private readonly Func<object, bool> _predicate;
        private readonly string _modifier;

        public PredicatePath(IReadOnlyList<IPath> selector, string op, object value, string modifier)
        {
            _selector = selector;
            _value = value;
            _modifier = modifier;
            _predicate = GetPredicateFunc(op, value);
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            // For each object, apply the selector
            // For each value in the selector result, compare to the value
            // return the object if and only iff all selector values satisfy the comparison

            return input.Where(SatisfiesPredicate);
        }

        private bool SatisfiesPredicate(object obj)
        {
            var values = _selector.Filter(new[] { obj }).ToList();
            switch (_modifier)
            {
                // all, at least one (default)
                case "": return values.Count >= 1 && values.All(value => _predicate(value));
                // all, if any
                case "*": return values.Count == 0 || values.All(value => _predicate(value));
                // at least one
                case "+": return values.Count >= 1 && values.Count(value => _predicate(value)) >= 1;
                // exactly one
                case "|": return values.Count >= 1 && values.Count(value => _predicate(value)) == 1;
                default: throw new InvalidOperationException($"Unknown modifier {_modifier}");
            }
        }

        private Func<object, bool> GetPredicateFunc(string op, object value)
        {
            switch (op)
            {
                case "=" when value == null:
                    return a => a == null;
                case "=":
                    return a => a != null && _value.Equals(a);
                default:
                    throw new InvalidOperationException($"Operator {op} is not a valid comparison");
            }
        }
    }
}
