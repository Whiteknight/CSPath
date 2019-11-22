using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSPath.Paths.Values;

namespace CSPath.Paths
{
    /// <summary>
    /// Returns a list of all public property values for each input object
    /// </summary>
    public class AllPropertiesPath : IPath
    {
        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input) 
            => input.SelectMany(GetPropertyValues);

        private static IEnumerable<IValueWrapper> GetPropertyValues(IValueWrapper obj)
        {
            if (obj.Value == null)
                return Enumerable.Empty<IValueWrapper>();
            var type = obj.Value.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var values = properties.Select(prop =>
                new AttributedValueWrapper(
                    prop.GetValue(obj.Value),
                    prop.GetCustomAttributes()
                )
            );
            return values;
        }
    }
}