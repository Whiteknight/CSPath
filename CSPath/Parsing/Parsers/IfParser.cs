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

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            return _predicate(t) ? _parser.Parse(t) : (false, default);
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);
    }
}