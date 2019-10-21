using System.Collections.Generic;
using System.Linq;

namespace CSPath
{
    public class PathTraverseContext
    {
        private readonly IReadOnlyList<IPathStage> _stages;


        public PathTraverseContext(IReadOnlyList<IPathStage> stages)
        {
            _stages = stages;
        }

        public IEnumerable<object> Traverse(object o) => Traverse(new[] { o });

        private IEnumerable<object> Traverse(IEnumerable<object> objects)
        {
            var current = objects;
            foreach (var stage in _stages)
            {
                current = stage.Filter(current);
            }

            return current;
        }
    }
}
