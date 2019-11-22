namespace CSPath.Paths.Values
{
    public struct SimpleValueWrapper : IValueWrapper
    {
        public SimpleValueWrapper(object value)
        {
            Value = value;
        }

        public object Value { get; }
    }
}
