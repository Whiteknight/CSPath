using System.Collections.Generic;

namespace CSPath.Paths
{
    public interface IPath
    {
        IEnumerable<object> Filter(IEnumerable<object> input);
    }
}