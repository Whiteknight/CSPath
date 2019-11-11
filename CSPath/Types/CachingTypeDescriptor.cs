using System;
using System.Collections.Generic;

namespace CSPath.Types
{
    public class CachingTypeDescriptor : ITypeDescriptor
    {
        private readonly ITypeDescriptor _descriptor;
        private readonly Dictionary<Type, bool> _cache;

        public CachingTypeDescriptor(ITypeDescriptor descriptor)
        {
            _descriptor = descriptor;
            _cache = new Dictionary<Type, bool>();
        }

        public bool IsMatch(Type type)
        {
            if (_cache.ContainsKey(type))
                return _cache[type];
            var isMatch = _descriptor.IsMatch(type);
            _cache.Add(type, isMatch);
            return isMatch;
        }
    }
}