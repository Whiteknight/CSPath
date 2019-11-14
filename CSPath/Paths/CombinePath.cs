using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    /// <summary>
    /// Concatenates the results of two or more paths together into a single output. May contain repeated
    /// values if the paths return the same things
    /// </summary>
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

        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
        {
            var inputAsList = input.ToList();
            return _paths.SelectMany(p => p.Filter(inputAsList));
        }
    }
}