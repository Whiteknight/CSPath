using System;

namespace CSPath.Parsing.Parsers
{
    public class OptionalParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IParser<TInput, TOutput> _parser;
        private readonly Func<TOutput> _getDefault;

        public OptionalParser(IParser<TInput, TOutput> parser, Func<TOutput> getDefault)
        {
            _parser = parser;
            _getDefault = getDefault;
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var result = _parser.Parse(t);
            if (result.Success)
                return result;
            TOutput value = default;
            if (_getDefault != null)
                value = _getDefault();
            return new Result<TOutput>(true, value);
        }
    }
}
