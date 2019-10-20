using System.Collections.Generic;

namespace CSPath
{
    public interface IPathStage
    {
        IEnumerable<object> Filter(IEnumerable<object> input);
    }
}