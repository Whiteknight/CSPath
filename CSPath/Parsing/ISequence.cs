namespace CSPath.Parsing
{
    /// <summary>
    /// Iterator pattern implementation with putback
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISequence<T>
    {
        /// <summary>
        /// Get the next input item or a default value
        /// </summary>
        /// <returns></returns>
        T GetNext();
        
        /// <summary>
        /// Put back an item onto the front of the stream so the next call to GetNext() will
        /// return it
        /// </summary>
        /// <param name="c"></param>
        void PutBack(T c);

        /// <summary>
        /// Get the next input item from the stream but leave it on the front of the stream
        /// so the next call to Peek() or GetNext() will return it.
        /// </summary>
        /// <returns></returns>
        T Peek();

        /// <summary>
        /// Returns true if the sequence is out of values, false otherwise
        /// </summary>
        bool IsAtEnd { get; }
    }
}