using System;
using System.Collections.Generic;

namespace CSPath.Parsing.Parsers
{
    public class SeparatedListParser<TInput, TItem, TSeparator, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IParser<TInput, TItem> _itemParser;
        private readonly IParser<TInput, TSeparator> _sepParser;
        private readonly Func<IReadOnlyList<TItem>, TOutput> _produce;
        private readonly bool _atLeastOne;

        public SeparatedListParser(IParser<TInput, TItem> itemParser, IParser<TInput, TSeparator> sepParser, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne)
        {
            _itemParser = itemParser;
            _sepParser = sepParser;
            _produce = produce;
            _atLeastOne = atLeastOne;
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            var items = GetItems(t);
            return _atLeastOne && items.Count == 0 ? (false, default) : (true, _produce(items));
        }

        private List<TItem> GetItems(ISequence<TInput> t)
        {
            var items = new List<TItem>();
            var (success, value) = _itemParser.Parse(t);
            if (!success)
                return items;

            items.Add(value);
            while (true)
            {
                (success, _) = _sepParser.Parse(t);
                if (!success)
                    break;

                (success, value) = _itemParser.Parse(t);
                if (!success)
                    throw new Exception("Incomplete separated list");

                items.Add(value);
            }

            return items;
        }
    }
}