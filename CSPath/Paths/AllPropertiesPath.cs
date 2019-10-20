using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSPath.Paths
{
    public class AllPropertiesPath : IPathStage
    {
        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return input.SelectMany(GetPropertyValues);
        }

        private IEnumerable<object> GetPropertyValues(object obj)
        {
            if (obj == null)
                return Enumerable.Empty<object>();
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var values = properties.Select(prop => prop.GetValue(obj));
            return values;
        }
    }
}