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

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            foreach (var parser in _parsers)
            {
                var (success, value) = parser.Parse(t);
                if (success)
                    return (true, value);
            }

            return (false, default);
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);
    }
}