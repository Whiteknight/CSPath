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
        {
            if (arg == null)
                return Enumerable.Empty<object>(); 
            var type = arg.GetType();
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return ((IEnumerable) arg).Cast<object>();
            return Enumerable.Empty<object>();
        }
    }
}
