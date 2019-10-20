using System;

namespace CSPath.Parsing.Parsers
{
    public class PredicateParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<TInput, bool> _predicate;
        private readonly Func<TInput, TOutput> _produce;

        public PredicateParser(Func<TInput, bool> predicate, Func<TInput, TOutput> produce)
        {
            _predicate = predicate;
            _produce = produce;
        }

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            if (_predicate(t.Peek()))
                return (true, _produce(t.GetNext()));
            return (false, default);
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);
    }
}