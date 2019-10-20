using System;
using System.Collections.Generic;

namespace CSPath.Parsing.Parsers
{
    public class ListParser<TInput, TItem, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IParser<TInput, TItem> _parser;
        private readonly Func<IReadOnlyList<TItem>, TOutput> _produce;
        private readonly bool _atLeastOne;

        public ListParser(IParser<TInput, TItem> parser, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne)
        {
            _parser = parser;
            _produce = produce;
            _atLeastOne = atLeastOne;
        }

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            var items = new List<TItem>();
            while (true)
            {
                var (success, value) = _parser.Parse(t);
                if (!success)
                    break;
                items.Add(value);
            }

            if (_atLeastOne && items.Count == 0)
                return (false, default);
            return (true, _produce(items));
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);
    }
}