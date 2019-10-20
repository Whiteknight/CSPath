using System.Collections.Generic;

namespace CSPath.Paths
{
    public class InstancePath : IPathStage
    {
        private readonly object _instance;

        public InstancePath(object instance)
        {
            _instance = instance;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return new[] { _instance };
        }
    }
}