using System.Collections.Generic;

namespace CSPath.Paths
{
    public class PipelinePath : IPathStage
    {
        private readonly IReadOnlyList<IPathStage> _stages;

        public PipelinePath(IReadOnlyList<IPathStage> stages)
        {
            _stages = stages;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            var current = input;
            foreach (var stage in _stages)
                current = stage.Filter(current);
            return current;
        }
    }
}