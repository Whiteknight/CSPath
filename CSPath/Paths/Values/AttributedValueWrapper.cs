using System;
using System.Collections.Generic;

namespace CSPath.Paths.Values
{
    public class AttributedValueWrapper : IValueWrapper
    {
        public AttributedValueWrapper(object value, IEnumerable<Attribute> attributes)
        {
            Attributes = attributes;
            Value = value;
        }

        public IEnumerable<Attribute> Attributes { get; }
        public object Value { get; }
    }
}