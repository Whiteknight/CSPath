using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSPath.Paths
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets values of all public properties for the given object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetPublicPropertyValues(this object obj)
        {
            if (obj == null)
                return Enumerable.Empty<IValueWrapper>();

            return obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(prop => prop.GetValue(obj));
        }
    }
}