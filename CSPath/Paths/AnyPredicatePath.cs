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

        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
        {
            if (_any)
                return input.Where(obj => ApplyFilter(obj).Any());
            return input.Where(obj => !_path.Filter(new[] { obj }).Any());
        }

        private IEnumerable<IValueWrapper> ApplyFilter(IValueWrapper obj) 
            => _path.Filter(new[] { obj }).ToList();
    }
}
