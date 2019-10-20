using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSPath.Parsing.Parsers;
using static CSPath.Parsing.Parsers.ParserMethods;
using static CSPath.Parsing.Tokenizing.TokenizerMethods;

namespace CSPath.Parsing.Tokenizing
{
    public static class LexicalGrammar
    {
        private static readonly HashSet<char> _hexDigits = new HashSet<char>("abcdefABCDEF0123456789");

        public static IParser<char, PathToken> GetParser()
        {
            var identifiers = Rule(
                If<char>(c => char.IsLetter(c) || c == '_'),
                List(
                    If<char>(c => char.IsLetter(c) || char.IsDigit(c) || c == '_'),
                    c => c.ToArray()
                ),
                (start, rest) => new PathToken(start.ToString() + new string(rest), TokenType.Identifier)
            );
            var chars = Rule(
                Match("'", c => c[0]),
                First(
                    // TODO: "\u" and "\U" escapes
                    Rule(
                        Match("\\x", c => c),
                        List(If<char>(c => _hexDigits.Contains(c)), t => ((char) int.Parse(new string(t.ToArray()))).ToString()),
                        (escape, c) => c
                    ),
                    Rule(
                        Match("\\", c => c),
                        Any<char, char>(c => c),
                        (escape, c) => c.ToString()
                    ),
                    If<char, string>(c => c != '\'', c => c.ToString()),
                    new ThrowExceptionParser<char, string>(t => $"Expected char value but found {t.Peek()}")
                ),
                Match("'", c => c[0]),
                (start, content, end) => new PathToken(content, TokenType.Character)
            );

            var strings = Rule(
                Match("\"", c => c[0]),
                List(
                    First(
                        Rule(
                            Match("\\", c => c[0]),
                            Any<char, char>(c => c),
                            (escape, c) => c.ToString()
                        ),
                        If<char, string>(c => c != '"', c => c.ToString())
                    ),
                    s => string.Join("", s)
                ),
                Match("\"", c => c[0]),
                (start, body, end) => new PathToken(body, TokenType.String)
            );

            return First(
                Match("\0", c => PathToken.EndOfInput()),
                List(If<char>(char.IsWhiteSpace), t => new PathToken(new string(t.ToArray()), TokenType.Whitespace), true),
                identifiers,
                Match(".", c => new PathToken(new string(c), TokenType.Dot)),
                Match("//", c => new PathToken(new string(c), TokenType.DoubleForwardSlash)),
                Match("/", c => new PathToken(new string(c), TokenType.ForwardSlash)),
                Match("*", c => new PathToken(new string(c), TokenType.Star)),
                Match("|", c => new PathToken(new string(c), TokenType.Bar)),
                Match("(", c => new PathToken(new string(c), TokenType.OpenParen)),
                Match(")", c => new PathToken(new string(c), TokenType.CloseParen)),
                Match("[", c => new PathToken(new string(c), TokenType.OpenBracket)),
                Match("]", c => new PathToken(new string(c), TokenType.CloseBracket)),
                Match(",", c => new PathToken(new string(c), TokenType.Comma)),
                Match("<", c => new PathToken(new string(c), TokenType.OpenAngle)),
                Match(">", c => new PathToken(new string(c), TokenType.CloseAngle)),
                If(t => char.IsNumber(t.Peek()) || t.Peek() == '-', new NumberParser()),
                chars,
                strings,
                new ThrowExceptionParser<char, PathToken>(t => $"Unexpected character {t.Peek()}")
            );
        }
    }
}