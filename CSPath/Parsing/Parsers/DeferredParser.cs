using System;

namespace CSPath.Parsing.Parsers
{
    public class DeferredParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<IParser<TInput, TOutput>> _getParser;

        public DeferredParser(Func<IParser<TInput, TOutput>> getParser)
        {
            _getParser = getParser;
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);

        public (bool success, TOutput value) Parse(ISequence<TInput> t) => _getParser().Parse(t);
    }
}
