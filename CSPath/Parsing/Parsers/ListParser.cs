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

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var items = new List<TItem>();
            while (true)
            {
                var result = _parser.Parse(t);
                if (!result.Success)
                    break;
                items.Add(result.Value);
            }

            if (_atLeastOne && items.Count == 0)
                return Result<TOutput>.Fail();
            return new Result<TOutput>(true, _produce(items));
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}