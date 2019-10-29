using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSPath.Paths
{
    /// <summary>
    /// Gets the value of a single public property by name, if it exists. Nothing otherwise
    /// </summary>
    public class NamedPropertyPath : IPath
    {
        private readonly string _name;

        public NamedPropertyPath(string name)
        {
            _name = name;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return input
                .Where(i => i != null)
                .Select(i => new
                {
                    Object = i,
                    Property = i.GetType().GetProperty(_name, BindingFlags.Public | BindingFlags.Instance)
                })
                .Where(x => x.Property != null)
                .Select(x => x.Property.GetValue(x.Object));
        }
    }
}