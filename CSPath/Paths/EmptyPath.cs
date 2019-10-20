using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class EmptyPath : IPathStage
    {
        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return Enumerable.Empty<object>();
        }
    }
}