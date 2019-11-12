using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Predicates
{
    public class AllIfAnyArityComparer : IArityComparer
    {
        public bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate) 
            => values.Count == 0 || values.All(predicate);
    }
}
