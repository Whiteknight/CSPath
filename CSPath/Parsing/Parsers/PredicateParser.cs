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

        public IParseResult<TOutput> Parse(ISequence<TInput> t) 
            => _predicate(t.Peek()) ? new Result<TOutput>(true, _produce(t.GetNext())) : Result<TOutput>.Fail();

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}