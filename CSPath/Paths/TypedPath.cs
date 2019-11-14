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
        {
            // TODO: Value wrapper to include type information?
            return input.Where(o => _decriptor.IsMatch(o.Value.GetType()));
        }
    }
}