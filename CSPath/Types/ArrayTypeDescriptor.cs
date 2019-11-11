using System;

namespace CSPath.Types
{
    public class ArrayTypeDescriptor : ITypeDescriptor
    {
        private readonly ITypeDescriptor _elementType;
        private readonly int _dimensions;

        public ArrayTypeDescriptor(ITypeDescriptor elementType, int dimensions)
        {
            _elementType = elementType;
            _dimensions = dimensions;
        }

        public bool IsMatch(Type type)
        {
            if (!type.IsArray || !type.HasElementType)
                return false;
            if (type.GetArrayRank() != _dimensions)
                return false;
            var elementType = type.GetElementType();
            return _elementType.IsMatch(elementType);
        }
    }
}