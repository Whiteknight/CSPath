using System;

namespace CSPath.Types
{
    /// <summary>
    /// Contains a matching algorithm for a Type.
    /// </summary>
    public interface ITypeDescriptor
    {
        /// <summary>
        /// Determines if the given type matches the known requirements. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsMatch(Type type);
    }
}