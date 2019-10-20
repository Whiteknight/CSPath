using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSPath.Paths
{
    public static class ObjectExtensions
    {
        public static IEnumerable<object> GetPublicPropertyValues(this object obj)
        {
            if (obj == null)
                return Enumerable.Empty<object>();

            return obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(prop => prop.GetValue(obj));
        }
    }
}