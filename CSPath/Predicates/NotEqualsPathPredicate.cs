namespace CSPath.Predicates
{
    public class NotEqualsPathPredicate : IPathPredicate
    {
        private readonly object _value;

        public NotEqualsPathPredicate(object value)
        {
            _value = value;
        }

        public bool Test(object obj)
        {
            if (obj == null && _value == null)
                return false;
            if (obj == null || _value == null)
                return true;
            return !obj.Equals(_value);
        }
    }
}