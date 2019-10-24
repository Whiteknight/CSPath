namespace CSPath.Parsing
{
    // Sequence is basically an Iterator pattern with a Putback
    // (don't know if there's a better name for it)
    public interface ISequence<T>
    {
        T GetNext();
        void PutBack(T c);
        T Peek();
        bool IsAtEnd { get; }
    }
}