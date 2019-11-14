using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSPath.Paths.Values;

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

        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
        {
            return input
                .Where(i => i.Value != null)
                .Select(i => new
                {
                    Object = i.Value,
                    Property = i.Value.GetType().GetProperty(_name, BindingFlags.Public | BindingFlags.Instance)
                })
                .Where(x => x.Property != null)
                .Select(x => (IValueWrapper)new AttributedValueWrapper(
                    x.Property.GetValue(x.Object),
                    x.Property.GetCustomAttributes()
                ));
        }
    }
}