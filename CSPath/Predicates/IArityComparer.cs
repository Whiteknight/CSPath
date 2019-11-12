using System;
using System.Collections.Generic;

namespace CSPath.Predicates
{
    public interface  IArityComparer
    {
        bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate);
    }
}