using System;

namespace CSPath.Parsing.Parsers
{
    public class TransformParser<TInput, TOutput, TTransform> : IParser<TInput, TTransform>
    {
        private readonly IParser<TInput, TOutput> _parser;
        private readonly Func<TOutput, TTransform> _produce;

        public TransformParser(IParser<TInput, TOutput> parser, Func<TOutput, TTransform> produce)
        {
            _parser = parser;
            _produce = produce;
        }

        public (bool success, object value) ParseUntyped(ISequence<TInput> t) => Parse(t);

        public (bool success, TTransform value) Parse(ISequence<TInput> t)
        {
            var (success, value) = _parser.Parse(t);
            if (!success)
                return (false, default);
            return (true, _produce(value));
        }
    }
}
