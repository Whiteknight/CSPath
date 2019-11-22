using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Predicates
{
    public class ExactlyArityComparer : IArityComparer
    {
        private readonly int _number;

        public ExactlyArityComparer(int number)
        {
            _number = number;
        }

        // TODO: Would like a variant where "the List contains exactly N items and all N match"
        public bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate)
            => values.Count(predicate) == _number;
    }
}