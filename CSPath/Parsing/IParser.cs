namespace CSPath.Parsing
{
    public interface IParser
    {
        //string Name { get; set; }
        //IParser Accept(IParserVisitorImplementation visitor);
        //IEnumerable<IParser> GetChildren();
        //IParser ReplaceChild(IParser find, IParser replace);
    }

    public interface IParser<TInput> : IParser
    {
        IParseResult<object> ParseUntyped(ISequence<TInput> t);
    }

    public interface IParser<TInput, out TOutput> : IParser<TInput>
    {
        IParseResult<TOutput> Parse(ISequence<TInput> t);
    }

    public interface IParseResult<out TOutput>
    {
        bool Success { get; }
        TOutput Value { get; }

        IParseResult<object> Untype();
    }

    public struct Result<TOutput> : IParseResult<TOutput>
    {
        public Result(bool success, TOutput value)
        {
            Success = success;
            Value = value;
        }

        public bool Success { get; }
        public TOutput Value { get; }

        public IParseResult<object> Untype() => new Result<object>(Success, Value);

        public static Result<TOutput> Fail() => new Result<TOutput>(false, default);
    }
}