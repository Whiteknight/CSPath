using System.Linq;

namespace CSPath.Parsing.Tokenizing
{
    public class PathToken
    {
        public PathToken(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }

        public string Value { get; }
        public TokenType Type { get; }

        public static PathToken EndOfInput() => new PathToken(null, TokenType.EndOfInput);
        public static PathToken Whitespace(string s) => new PathToken(s, TokenType.Whitespace);

        public bool IsType(params TokenType[] types) => types.Any(t => Type == t);

        public bool Is(TokenType type, string value) => Type == type && Value == value;

        public override string ToString()
        {
            return $"{Type}:{Value}";
        }
    }
}