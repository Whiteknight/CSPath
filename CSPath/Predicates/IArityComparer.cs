using System;
using System.Collections.Generic;

namespace CSPath.Predicates
{
    /// <summary>
    /// Compares the arity (count) of a result set to see if it satisfies a given criteria
    /// </summary>
    public interface  IArityComparer
    {
        /// <summary>
        /// Filter the input values according to the provided predicate. Returns true if
        /// the result set has the necessary arity (count). Returns false
        /// otherwise.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate);
    }
}