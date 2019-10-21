using System.Collections;
using System.Collections.Generic;

namespace CSPath.Parsing.Tokenizing
{
    public class Tokenizer : IEnumerable<PathToken>, ISequence<PathToken>
    {
        private readonly ISequence<char> _chars;
        private readonly Stack<PathToken> _putbacks;
        private readonly IParser<char, PathToken> _scanner;

        public Tokenizer(string s)
            : this(new StringCharacterSequence(s ?? ""))
        {
        }

        public Tokenizer(ISequence<char> chars)
        {
            _chars = chars;
            _putbacks = new Stack<PathToken>();

            // TODO: Inject this
            _scanner = LexicalGrammar.GetParser();
        }

        public PathToken GetNext()
        {
            if (_putbacks.Count > 0)
                return _putbacks.Pop();

            while (true)
            {
                var (success, value) = _scanner.Parse(_chars);
                if (!success || value == null)
                    return PathToken.EndOfInput();
                if (value.IsType(TokenType.Whitespace))
                    continue;
                return value;
            }
        }

        public void PutBack(PathToken pathToken)
        {
            if (pathToken != null)
                _putbacks.Push(pathToken);
        }

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