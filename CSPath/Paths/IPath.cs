using System.Collections.Generic;

namespace CSPath
{
    public interface IPath
    {
        IEnumerable<object> Filter(IEnumerable<object> input);
    }
}