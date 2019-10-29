using System.Collections.Generic;

namespace CSPath.Paths
{
    /// <summary>
    /// A single stage in a path pipeline. Takes a sequence of object values and returns a sequence of
    /// object values according to the rules of this stage.
    /// </summary>
    public interface IPath
    {
        IEnumerable<object> Filter(IEnumerable<object> input);
    }
}