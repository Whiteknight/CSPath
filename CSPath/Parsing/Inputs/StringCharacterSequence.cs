using System.Collections.Generic;
using System.Linq;

namespace CSPath.Parsing.Inputs
{
    public class StringCharacterSequence : ISequence<char>
    {
        private readonly string _s;
        private readonly Stack<char> _putbacks;
        private int _index;

        public StringCharacterSequence(string s)
        {
            _s = s;
            _putbacks = new Stack<char>();
        }

        public char GetNext()
        {
            if (_putbacks.Any())
                return _putbacks.Pop();
            if (_index >= _s.Length)
                return '\0';
            var next = _s[_index++];

            return next;
        }

        public void PutBack(char c) => _putbacks.Push(c);

        public char Peek()
        {
            if (_putbacks.Count > 0)
                return _putbacks.Peek();
            if (IsAtEnd)
                return '\0';
            return _s[_index];
        }

        public bool IsAtEnd => _index >= _s.Length && _putbacks.Count == 0;
    }
}