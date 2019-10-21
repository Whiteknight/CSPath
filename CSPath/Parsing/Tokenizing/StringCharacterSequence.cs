using System.Collections.Generic;
using System.Linq;

namespace CSPath.Parsing.Tokenizing
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
    }
}