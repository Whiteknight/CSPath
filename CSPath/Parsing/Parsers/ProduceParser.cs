using System;

namespace CSPath.Parsing.Parsers
{
    public class ProduceParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<TOutput> _produce;

        public ProduceParser(Func<TOutput> produce)
        {
            _produce = produce;
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);

        public (bool success, TOutput value) Parse(ISequence<TInput> t) => (true, _produce());
    }
}
