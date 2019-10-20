using System;
using System.Collections.Generic;

namespace CSPath.Parsing.Parsers
{
    public class RuleParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IReadOnlyList<IParser<TInput>> _parsers;
        private readonly Func<IReadOnlyList<object>, TOutput> _produce;

        public RuleParser(IReadOnlyList<IParser<TInput>> parsers, Func<IReadOnlyList<object>, TOutput> produce)
        {
            _parsers = parsers;
            _produce = produce;
        }

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            var window = new WindowSequence<TInput>(t);
            var outputs = new object[_parsers.Count];
            for (int i = 0; i < _parsers.Count; i++)
            {
                var (success, value) = _parsers[i].ParseUntyped(window);
                if (!success)
                {
                    window.Rewind();
                    return (false, default);
                }

                outputs[i] = value;
            }

            return (true, _produce(outputs));
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);
    }
}