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

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();

        public IParseResult<TOutput> Parse(ISequence<TInput> t) => new Result<TOutput>(true, _produce());
    }
}
