using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Predicates
{
    public class AllAtLeastOneArityComparer : IArityComparer
    {
        public bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate)
            => values.Count >= 1 && values.All(predicate);
    }
}