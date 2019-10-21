using System;

namespace CSPath.Parsing
{
    // Sequence is basically an Iterator pattern with a Putback
    // (don't know if there's a better name for it)
    public interface ISequence<T>
    {
        T GetNext();
        void PutBack(T c);
    }

    public static class SequenceExtensions
    {
        public static T Peek<T>(this ISequence<T> cs)
        {
            var c = cs.GetNext();
            cs.PutBack(c);
            return c;
        }

        public static T Expect<T>(this ISequence<T> cs, T expected)
        {
            var c = cs.GetNext();
            if (!c.Equals(expected))
                throw new Exception("Expected");
            return c;
        }
    }
}