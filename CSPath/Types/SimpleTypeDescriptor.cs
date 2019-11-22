using System;
using System.Collections.Generic;

namespace CSPath.Types
{
    public class SimpleTypeDescriptor : ITypeDescriptor
    {
        private static readonly IReadOnlyDictionary<string, string> _keywordTypeMap = new Dictionary<string, string>
        {
            { "bool", "Boolean" },
            { "byte", "Byte" },
            { "char", "Char" },
            { "decimal", "Decimal" },
            { "double", "Double" },
            { "float", "Float" },
            { "int", "Int32" },
            { "long", "Int64" },
            { "sbyte", "SByte" },
            { "short", "Int16" },
            { "uint", "UInt32" },
            { "ulong", "UInt64" },
            { "ushort", "UInt16" }
        };

        public string Name { get; }

        public SimpleTypeDescriptor(string name)
        {
            var lowerName = name.ToLowerInvariant();
            Name = _keywordTypeMap.ContainsKey(lowerName) ? _keywordTypeMap[lowerName] : name;
        }

        public bool IsMatch(Type type) => type.Name.Split('`')[0].Equals(Name, StringComparison.OrdinalIgnoreCase);
    }
}
