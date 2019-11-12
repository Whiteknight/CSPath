using System.Collections.Generic;

namespace CSPath.Paths
{
    public class GroupedPath : IPath
    {
        private readonly IReadOnlyList<IPath> _path;

        public GroupedPath(IReadOnlyList<IPath> path)
        {
            _path = path;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return _path.Filter(input);
        }
    }
}