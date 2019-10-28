using System.Collections.Generic;

namespace CSPath.Parsing.Parsers
{
    public class FirstParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IReadOnlyList<IParser<TInput, TOutput>> _parsers;

        public FirstParser(params IParser<TInput, TOutput>[] parsers)
        {
            _parsers = parsers;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            foreach (var parser in _parsers)
            {
                var result = parser.Parse(t);
                if (result.Success)
                    return result;
            }

            return Result<TOutput>.Fail();
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}