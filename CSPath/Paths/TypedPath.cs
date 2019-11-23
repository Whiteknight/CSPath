using System;
using System.Collections.Generic;
using System.Linq;
using CSPath.Types;

namespace CSPath.Paths
{
    /// <summary>
    /// Returns only values of the given type, determined by type name string.
    /// If the type name contains any "." characters, it is considered to be a fully-qualified name.
    /// Otherwise it is considered to be a short name.
    /// </summary>
    public class TypedPath : IPath
    {
        private readonly ITypeDescriptor _decriptor;

        public TypedPath(ITypeDescriptor decriptor)
        {
            _decriptor = decriptor;
        }

        public IEnumerable<IValueWrapper> Filter(IEnumerable<IValueWrapper> input)
            => input.Where(IsTypeMatch);

        private bool IsTypeMatch(IValueWrapper o)
        {
            var workingSet = new Stack<Type>();
            var current = o.Value.GetType();
            if (_decriptor.IsMatch(current))
                return true;

            workingSet.Push(current.BaseType);
            foreach (var iface in current.GetInterfaces())
                workingSet.Push(iface);

            // We don't need to de-dupe types here, because the _descriptor does caching
            while (workingSet.Count > 0)
            {
                current = workingSet.Pop();
                if (current == null)
                    continue;

                if (_decriptor.IsMatch(current))
                    return true;

                workingSet.Push(current.BaseType);
                foreach (var iface in current.GetInterfaces())
                    workingSet.Push(iface);
            }

            return false;
        }
    }
}