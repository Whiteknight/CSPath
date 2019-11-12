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

        public bool IsMatch(IReadOnlyList<object> values, Func<object, bool> predicate)
            => values.Count(predicate) == _number;
    }
}