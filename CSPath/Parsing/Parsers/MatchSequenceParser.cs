using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Parsing.Parsers
{
    public class MatchSequenceParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly TInput[] _find;
        private readonly Func<TInput[], TOutput> _produce;

        public MatchSequenceParser(IEnumerable<TInput> find, Func<TInput[], TOutput> produce)
        {
            _find = find.ToArray();
            _produce = produce;
        }

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            if (_find.Length == 1)
            {
                if (t.Peek().Equals(_find[0]))
                    return (true,  _produce(new [] { t.GetNext() }));
                return (false, default);
            }

            var buffer = new TInput[_find.Length];
            for (int i = 0; i < _find.Length; i++)
            {
                var c = t.GetNext();
                buffer[i] = c;
                if (!c.Equals(_find[i]))
                {
                    for (; i >= 0; i--)
                        t.PutBack(buffer[i]);
                    return (false, default);
                }
            }

            return (true, _produce(buffer));
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);
    }
}