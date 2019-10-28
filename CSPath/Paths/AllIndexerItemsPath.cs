using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
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
