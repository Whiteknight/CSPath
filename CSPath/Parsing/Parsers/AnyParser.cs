using System;

namespace CSPath.Parsing.Parsers
{
    public class AnyParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> _produce;

        public AnyParser(Func<TInput, TOutput> produce)
        {
            _produce = produce;
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            var result = t.GetNext();
            if (!t.IsValid(result))
                return (false, default);
            return (true, _produce(result));
        }
    }
}