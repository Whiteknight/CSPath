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

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var items = GetItems(t);
            return _atLeastOne && items.Count == 0 ? Result<TOutput>.Fail() : new Result<TOutput>(true, _produce(items));
        }

        private List<TItem> GetItems(ISequence<TInput> t)
        {
            var items = new List<TItem>();
            var result = _itemParser.Parse(t);
            if (!result.Success)
                return items;

            items.Add(result.Value);
            while (true)
            {
                var separatorResult = _sepParser.Parse(t);
                if (!separatorResult.Success)
                    break;

                var nextResult = _itemParser.Parse(t);
                if (!nextResult.Success)
                    throw new Exception("Incomplete separated list");

                items.Add(nextResult.Value);
            }

            return items;
        }
    }
}