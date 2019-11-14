using System.Collections.Generic;

namespace CSPath
{
    /// <summary>
    /// A single stage in a path pipeline. Takes a sequence of object values and returns a sequence of
    /// object values according to the rules of this stage.
    /// </summary>
    public interface IPath
    {
        IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input);
    }
}