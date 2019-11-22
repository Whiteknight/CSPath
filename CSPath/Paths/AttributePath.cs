using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSPath.Paths.Values;

namespace CSPath.Paths
{
    public class AttributePath : IPath
    {
        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
        {
            return input.SelectMany(GetAttributes);
        }

        private IEnumerable<IValueWrapper> GetAttributes(IValueWrapper obj)
        {
            if (obj is AttributedValueWrapper avw)
                return avw.Attributes.Select(a => (IValueWrapper)new SimpleValueWrapper(a));
            // TODO: Do we want the rest of this logic? It seems like it won't come up much
            if (obj.Value is ICustomAttributeProvider attrProvider)
                return attrProvider.GetCustomAttributes(true).OfType<Attribute>().Select(a => (IValueWrapper) new SimpleValueWrapper(a));
            return obj.GetType().GetCustomAttributes(true).OfType<Attribute>().Select(a => (IValueWrapper)new SimpleValueWrapper(a));
        }
    }
}