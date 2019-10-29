using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    /// <summary>
    /// Treats the object as an IEnumerable and returns all objects from it.
    /// If the object is not an IEnumerable, returns nothing.
    /// </summary>
    public class AllIndexerItemsPath : IPath
    {
        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return input.SelectMany(GetAllIndexerItems);
        }

        private static IEnumerable<object> GetAllIndexerItems(object arg) 
            => arg is IEnumerable enumerable ? enumerable.Cast<object>() : Enumerable.Empty<object>();
    }
}
