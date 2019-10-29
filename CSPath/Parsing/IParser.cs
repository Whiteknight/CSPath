namespace CSPath.Parsing
{
    public interface IParser
    {
    }

    /// <summary>
    /// Parser class takes a sequence of TInput and outputs productions of unknown type
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IParser<TInput> : IParser
    {
        IParseResult<object> ParseUntyped(ISequence<TInput> t);
    }

    /// <summary>
    /// Parser class takes a sequence of TInput and outputs productions of type TOutput
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IParser<TInput, out TOutput> : IParser<TInput>
    {
        IParseResult<TOutput> Parse(ISequence<TInput> t);
    }

    /// <summary>
    /// Parser result objects, contains a success/fail flag and the production value
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public interface IParseResult<out TOutput>
    {
        bool Success { get; }
        TOutput Value { get; }

        /// <summary>
        /// Convert the return value explicitly to object
        /// (cast to IParseResult of object fails when TOutput is a struct/primitive type and boxing is involved)
        /// </summary>
        /// <returns></returns>
        IParseResult<object> Untype();
    }

    /// <summary>
    /// Represents a parse failure
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public struct FailResult<TOutput> : IParseResult<TOutput>
    {
        public bool Success => false;
        public TOutput Value => default;

        public IParseResult<object> Untype() => new FailResult<object>();
    }

    /// <summary>
    /// Represents a successful parse production
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public struct SuccessResult<TOutput> : IParseResult<TOutput>
    {
        public SuccessResult(TOutput value)
        {
            Value = value;
        }

        public bool Success => true;
        public TOutput Value { get; }

        public IParseResult<object> Untype() => new SuccessResult<object>(Value);
    }
}