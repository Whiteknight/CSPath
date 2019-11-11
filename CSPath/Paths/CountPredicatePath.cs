using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class CountPredicatePath : IPath
    {
        private readonly IReadOnlyList<IPath> _path;
        private readonly int _limit;
        private readonly string _func;

        public CountPredicatePath(IReadOnlyList<IPath> path, int limit, string func)
        {
            _path = path;
            _limit = limit;
            _func = func;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            switch (_func)
            {
                case "atleast":
                    return input.Where(obj => _path.Filter(new[] { obj }).Count() >= _limit);
                case "atmost":
                    return input.Where(obj => _path.Filter(new[] { obj }).Count() <= _limit);
                case "exactly":
                    return input.Where(obj => _path.Filter(new[] { obj }).Count() == _limit);
                default:
                    throw new InvalidOperationException($"Unknown count function {_func}");
            }
        }
    }
}