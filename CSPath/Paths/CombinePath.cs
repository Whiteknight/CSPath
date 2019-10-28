using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class CombinePath : IPath
    {
        private readonly IReadOnlyList<IReadOnlyList<IPath>> _paths;

        public CombinePath(IReadOnlyList<IReadOnlyList<IPath>> paths)
        {
            _paths = paths;
        }

        public CombinePath(IEnumerable<IPath> paths)
        {
            _paths = paths.Select(p => (IReadOnlyList<IPath>) new List<IPath> { p }).ToList();
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            var inputAsList = input.ToList();
            return _paths.SelectMany(p => p.Filter(inputAsList));
        }
    }
}