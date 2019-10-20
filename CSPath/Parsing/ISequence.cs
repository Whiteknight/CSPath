using System;

namespace CSPath.Parsing
{
    public interface ISequence<T>
    {
        T GetNext();
        void PutBack(T c);
        bool IsValid(T c);
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