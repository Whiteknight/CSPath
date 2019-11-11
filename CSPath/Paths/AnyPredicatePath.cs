using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class AnyPredicatePath : IPath
    {
        private readonly IReadOnlyList<IPath> _path;
        private readonly bool _any;

        public AnyPredicatePath(IReadOnlyList<IPath> path, bool any)
        {
            _path = path;
            _any = any;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            if (_any)
                return input.Where(obj => ApplyFilter(obj).Any());
            return input.Where(obj => !_path.Filter(new[] { obj }).Any());
        }

        private IEnumerable<object> ApplyFilter(object obj)
        {
            var result = _path.Filter(new [] { obj }).ToList();
            return result;
        }
    }
}
