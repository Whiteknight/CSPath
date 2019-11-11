using System;

namespace CSPath.Types
{
    public interface ITypeDescriptor
    {
        bool IsMatch(Type type);
    }
}