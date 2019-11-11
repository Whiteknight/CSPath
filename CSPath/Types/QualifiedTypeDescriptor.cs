using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Types
{
    // TODO: This class is a flithy mess. See if we can simplify the logic anywhere
    public class QualifiedTypeDescriptor : ITypeDescriptor
    {
        private readonly IReadOnlyList<ITypeDescriptor> _childDescriptors;

        public QualifiedTypeDescriptor(IReadOnlyList<ITypeDescriptor> childDescriptors)
        {
            // A type defined in C# as "A<B>.C<D>" will be represented via reflection as "A<>.C<B, D>"
            // Go over the list of type descriptors and try to transform it into that form, so we can match
            // it later.

            var cd = new List<ITypeDescriptor>();
            var typeArgs = new List<ITypeDescriptor>();
            int descriptorsLastIdx = childDescriptors.Count - 1;
            bool seenGeneric = false;

            // Loop through descriptors. If we see a generic type, we need to start collecting args and pushing
            // them forward. We need to replace GenericTypeDescriptors with GenericDefTypeDescriptors.
            // SimpleTypeDescriptors can be copied as-is.
            for (int i = 0; i < descriptorsLastIdx; i++)
            {
                var descriptor = childDescriptors[i];
                if (descriptor is GenericTypeDescriptor gtd)
                {
                    seenGeneric = true;
                    typeArgs.AddRange(gtd.TypeArguments);
                    cd.Add(new GenericDefTypeDescriptor(gtd.BaseType, gtd.TypeArguments.Count));
                }
                else
                    cd.Add(descriptor);
            }

            // When we get to the last descriptor, if we have collected typeArgs, we create a new 
            // GenericTypeDescriptor to hold all of them. Otherwise just add the last descriptor
            // as-is.
            var lastDescriptor = childDescriptors[descriptorsLastIdx];
            if (!seenGeneric && typeArgs.Count == 0)
                cd.Add(lastDescriptor);
            else if (lastDescriptor is SimpleTypeDescriptor std)
                cd.Add(new GenericTypeDescriptor(std, typeArgs));
            else if (lastDescriptor is GenericTypeDescriptor gtd)
            {
                typeArgs.AddRange(gtd.TypeArguments);
                cd.Add(new GenericTypeDescriptor(gtd.BaseType, typeArgs));
            }

            _childDescriptors = cd;
        }

        public bool IsMatch(Type type)
        {
            var current = type;
            int i = _childDescriptors.Count - 1;

            // We're going to have <namespaces>*.<types>*
            // So start from the end matching types. When we run out of declaring types,
            // we will switch over to matching namespace segments until we run out of child
            // descriptors

            for (; i >= 0; i--)
            {
                var descriptor = _childDescriptors[i];
                if (!descriptor.IsMatch(current))
                    return false;
                if (current.DeclaringType == null)
                {
                    i--;
                    break;
                }

                current = current.DeclaringType;
            }

            if (i == 0)
                return true;

            var namespaceParts = current.Namespace?.Split('.');
            if (namespaceParts == null)
                return false;

            int j = namespaceParts.Length - 1;
            if (i > j)
                return false;

            for (; i >= 0; i--)
            {
                var nsPart = namespaceParts[j];
                j--;
                var descriptor = _childDescriptors[i];
                if (descriptor is SimpleTypeDescriptor std)
                {
                    if (!std.Name.Equals(nsPart, StringComparison.OrdinalIgnoreCase))
                        return false;
                    continue;
                }
                return false;
            }

            return true;
        }

        // This is a synthetic type descriptor used by the QualifiedTypeDescriptor rewrite rules to 
        // help matching nested generic types
        public class GenericDefTypeDescriptor : ITypeDescriptor
        {
            private readonly ITypeDescriptor _baseType;
            private readonly int _numTypeArgs;

            public GenericDefTypeDescriptor(ITypeDescriptor baseType, int numTypeArgs)
            {
                _baseType = baseType;
                _numTypeArgs = numTypeArgs;
            }

            public bool IsMatch(Type type)
            {
                if (!_baseType.IsMatch(type))
                    return false;
                return type.GetGenericArguments().Count(c => c.IsGenericParameter) == _numTypeArgs;
            }
        }
    }
}