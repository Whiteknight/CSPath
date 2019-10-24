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

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            var (success, value) = _parser.Parse(t);
            if (success)
                return (true, value);
            value = default;
            if (_getDefault != null)
                value = _getDefault();
            return (true, value);
        }
    }
}
