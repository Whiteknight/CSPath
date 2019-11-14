using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CSPath.Paths.Values;

namespace CSPath.Paths
{
    /// <summary>
    /// Treats the object as an IEnumerable and returns all objects from it.
    /// If the object is not an IEnumerable, returns nothing.
    /// </summary>
    public class AllIndexerItemsPath : IPath
    {
        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
        {
            return input.SelectMany(GetAllIndexerItems);
        }

        private static IEnumerable<IValueWrapper> GetAllIndexerItems(IValueWrapper arg)
        {
            if (!(arg.Value is IEnumerable enumerable))
                return Enumerable.Empty<IValueWrapper>();
            return enumerable
                .Cast<object>()
                .Select(o => (IValueWrapper) new SimpleValueWrapper(o));
        }
    }
}
