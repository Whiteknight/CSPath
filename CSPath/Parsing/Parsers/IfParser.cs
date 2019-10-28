using System;

namespace CSPath.Parsing.Parsers
{
    public class IfParser<TInput, TOutput> : IParser<TInput, TOutput> 
    {
        private readonly Func<ISequence<TInput>, bool> _predicate;
        private readonly IParser<TInput, TOutput> _parser;

        public IfParser(Func<ISequence<TInput>, bool> predicate, IParser<TInput, TOutput> parser)
        {
            _predicate = predicate;
            _parser = parser;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t) 
            => _predicate(t) ? _parser.Parse(t) : Result<TOutput>.Fail();

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}