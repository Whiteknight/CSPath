using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public static class PathStageExtensions
    {
        public static IEnumerable<object> Filter(this IEnumerable<IPathStage> stages, IEnumerable<object> input)
        {
            if (stages == null || input == null)
                return Enumerable.Empty<object>();
            var current = input;
            foreach (var stage in stages)
                current = stage.Filter(current);
            return current;
        }
    }
}