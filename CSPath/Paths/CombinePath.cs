using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class CombinePath : IPathStage
    {
        private readonly IReadOnlyList<IReadOnlyList<IPathStage>> _paths;

        public CombinePath(IReadOnlyList<IReadOnlyList<IPathStage>> paths)
        {
            _paths = paths;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            var inputAsList = input.ToList();
            return _paths.SelectMany(p => FilterPath(p, inputAsList));
        }

        public IEnumerable<object> FilterPath(IReadOnlyList<IPathStage> path, IReadOnlyList<object> input)
        {
            IEnumerable<object> current = input;
            foreach (var stage in path)
                current = stage.Filter(current).ToList();
            return current;
        }
    }
}