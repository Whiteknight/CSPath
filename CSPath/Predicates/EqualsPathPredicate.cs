namespace CSPath.Predicates
{
    public class EqualsPathPredicate : IPathPredicate
    {
        private readonly object _value;

        public EqualsPathPredicate(object value)
        {
            _value = value;
        }

        public bool Test(object obj)
        {
            if (obj == null && _value == null)
                return true;
            if (obj == null || _value == null)
                return false;
            return obj.Equals(_value);
        }
    }
}