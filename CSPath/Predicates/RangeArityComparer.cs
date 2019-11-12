using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Predicates
{
    public class RangeArityComparer : IArityComparer
    {
        private readonly int _lowInclusive;
        private readonly int _highInclusive;

        public RangeArityComparer(int lowInclusive, int highInclusive)
        {
            _lowInclusive = lowInclusive;
            _highInclusive = highInclusive;
        }

        public bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate)
        {
            var count = values.Count(predicate);
            return count >= _lowInclusive && count <= _highInclusive;
        }
    }
}