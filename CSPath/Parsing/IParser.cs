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
        (bool success, object value) ParseUntyped(ISequence<TInput> t);
    }

    public interface IParser<TInput, TOutput> : IParser<TInput>
    {
        (bool success, TOutput value) Parse(ISequence<TInput> t);
    }
}