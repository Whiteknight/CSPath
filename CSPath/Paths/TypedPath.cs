using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    /// <summary>
    /// Returns only values of the given type, determined by type name string.
    /// If the type name contains any "." characters, it is considered to be a fully-qualified name.
    /// Otherwise it is considered to be a short name.
    /// </summary>
    public class TypedPath : IPath
    {
        private readonly string _typeName;
        private readonly bool _isFullyQualified;

        public TypedPath(string typeName)
        {
            _typeName = typeName;
            _isFullyQualified = typeName.Contains(".");
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            if (_isFullyQualified)
                return input.Where(i => (i.GetType().FullName ?? string.Empty).Equals(_typeName, StringComparison.OrdinalIgnoreCase));
            return input.Where(i => i.GetType().Name.Equals(_typeName, StringComparison.OrdinalIgnoreCase));
        }
    }
}