using System.Collections;
using System.Collections.Generic;

namespace CSPath.Parsing.Tokenizing
{
    public class Tokenizer : IEnumerable<PathToken>, ISequence<PathToken>
    {
        private readonly ISequence<char> _chars;
        private readonly Stack<PathToken> _putbacks;
        private readonly IParser<char, PathToken> _scanner;

        private bool _seenEnd;

        public Tokenizer(string s)
            : this(new StringCharacterSequence(s ?? ""), null)
        {
        }

        public Tokenizer(ISequence<char> chars, IParser<char, PathToken> parser)
        {
            _chars = chars;
            _putbacks = new Stack<PathToken>();

            // TODO: Inject this
            _scanner = parser ?? LexicalGrammar.GetParser();
            _seenEnd = false;
        }

        public PathToken GetNext()
        {
            if (_putbacks.Count > 0)
                return _putbacks.Pop();

            if (_seenEnd)
                return PathToken.EndOfInput();

            while (true)
            {
                var (success, value) = _scanner.Parse(_chars);
                if (!success || value == null)
                {
                    _seenEnd = true;
                    return PathToken.EndOfInput();
                }
                return value;
            }
        }

        public void PutBack(PathToken pathToken)
        {
            if (pathToken != null)
                _putbacks.Push(pathToken);
        }

        public PathToken Peek()
        {
            if (_putbacks.Count > 0)
                return _putbacks.Peek();
            if (_seenEnd)
                return PathToken.EndOfInput();

            var next = GetNext();
            PutBack(next);
            return next;
        }

        public bool IsAtEnd => _seenEnd && _putbacks.Count == 0;

        public IEnumerator<PathToken> GetEnumerator()
        {
            while (true)
            {
                var (success, value) = _scanner.Parse(_chars);
                if (!success || value == null || value.IsType(TokenType.EndOfInput))
                    break;
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}