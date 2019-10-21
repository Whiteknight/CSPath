using System.Collections.Generic;
using System.Reflection;

namespace CSPath.Paths
{
    public class NamedPropertyPath : IPath
    {
        private readonly string _name;

        public NamedPropertyPath(string name)
        {
            _name = name;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            foreach (var obj in input)
            {
                if (obj == null)
                    continue;
                var property = obj.GetType().GetProperty(_name, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    continue;
                yield return property.GetValue(obj);
            }
        }
    }
}