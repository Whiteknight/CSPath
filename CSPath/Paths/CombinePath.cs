using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class CombinePath : IPathStage
    {
        private readonly IPathStage _left;
        private readonly IPathStage _right;

        public CombinePath(IPathStage left, IPathStage right)
        {
            _left = left ?? new EmptyPath();
            _right = right ?? new EmptyPath();
            
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return _left.Filter(input).Concat(_right.Filter(input));
        }
    }
}