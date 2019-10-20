using System;

namespace CSPath.Parsing.Parsers
{
    public class ThrowExceptionParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<ISequence<TInput>, string> _getError;

        public ThrowExceptionParser(Func<ISequence<TInput>, string> getError)
        {
            _getError = getError;
        }

        public (bool success, TOutput value) Parse(ISequence<TInput> t)
        {
            throw new Exception(_getError(t));
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t)
        {
            throw new Exception(_getError(t));
        }
    }
}