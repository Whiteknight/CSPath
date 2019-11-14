using System;
using System.Collections.Generic;

namespace CSPath.Types
{
    public class GenericTypeDescriptor : ITypeDescriptor
    {
        public ITypeDescriptor BaseType { get; }
        public IReadOnlyList<ITypeDescriptor> TypeArguments { get; }

        public GenericTypeDescriptor(ITypeDescriptor baseType, IReadOnlyList<ITypeDescriptor> typeArgs)
        {
            BaseType = baseType;
            TypeArguments = typeArgs;
        }

        public bool IsMatch(Type type)
        {
            if (!BaseType.IsMatch(type))
                return false;
            if (!type.IsConstructedGenericType)
                return false;
            var typeArgs = type.GetGenericArguments();
            if (typeArgs.Length != TypeArguments.Count)
                return false;
            for (int i = 0; i < typeArgs.Length; i++)
            {
                if (!TypeArguments[i].IsMatch(typeArgs[i]))
                    return false;
            }

            return true;
        }
    }
}