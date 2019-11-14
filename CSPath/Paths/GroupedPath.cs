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

        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
        {
            return _path.Filter(input);
        }
    }
}