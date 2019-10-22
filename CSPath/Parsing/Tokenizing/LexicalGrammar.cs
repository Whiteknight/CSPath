using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing.Parsers;
using static CSPath.Parsing.Parsers.ParserMethods;
using static CSPath.Parsing.Tokenizing.TokenizerMethods;

namespace CSPath.Parsing.Tokenizing
{
    public static class LexicalGrammar
    {
        // TODO: Static instance of IParser so we only need to build it once

        private static readonly HashSet<char> _hexDigits = new HashSet<char>("abcdefABCDEF0123456789");

        public static IParser<char, PathToken> GetParser()
        {
            // identifier = (<char> | "_") (<char> | <digit> | "_")*
            var identifiers = Rule(
                Match<char>(c => char.IsLetter(c) || c == '_'),
                List(
                    Match<char>(c => char.IsLetter(c) || char.IsDigit(c) || c == '_'),
                    c => c.ToArray()
                ),
                (start, rest) => new PathToken(start.ToString() + new string(rest), TokenType.Identifier)
            );

            // chars = "'" ("\" "x" <hexdigit>+ | "\" . | .) "'"
            var chars = Rule(
                Match("'", c => c[0]),
                First(
                    // TODO: "\u" and "\U" escapes
                    Rule(
                        Match("\\x", c => c),
                        List(
                            Match<char>(c => _hexDigits.Contains(c)), 
                            t => ((char) int.Parse(new string(t.ToArray()))).ToString()
                        ),
                        (escape, c) => c
                    ),
                    Rule(
                        Match("\\", c => c),
                        Match<char>(c => c != '\0'),
                        (escape, c) => c.ToString()
                    ),
                    Match<char, string>(c => c != '\'', c => c.ToString()),
                    Error<char, string>(t => $"Expected char value but found {t.Peek()}")
                ),
                Match("'", c => c[0]),
                (start, content, end) => new PathToken(content, TokenType.Character)
            );

            // TODO: Do we want to support @ strings?
            // strings = """ ("\" . | .)* """
            var strings = Rule(
                Match("\"", c => c[0]),
                List(
                    First(
                        Rule(
                            Match("\\", c => c[0]),
                            Match<char>(c => c != '\0'),
                            (escape, c) => c.ToString()
                        ),
                        Match<char, string>(c => c != '"', c => c.ToString())
                    ),
                    s => string.Join("", s)
                ),
                Match("\"", c => c[0]),
                (start, body, end) => new PathToken(body, TokenType.String)
            );

            return First(
                // input char sequence returns "\0" for end-of-input. Detect that and return an EOI token
                Match("\0", c => PathToken.EndOfInput()),

                // Whitespace
                List(Match<char>(char.IsWhiteSpace), t => new PathToken(new string(t.ToArray()), TokenType.Whitespace), true),

                Match("true", c => new PathToken(null, TokenType.True)),
                Match("false", c => new PathToken(null, TokenType.False)),
                Match("null", c => new PathToken(null, TokenType.Null)),
                // Identifiers and names
                identifiers,

                // Symbols and operators
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
                Match("{", c => new PathToken(new string(c), TokenType.OpenBrace)),
                Match("}", c => new PathToken(new string(c), TokenType.CloseBrace)),
                Match("=", c => new PathToken(new string(c), TokenType.Equals)),
                Match("+", c => new PathToken(new string(c), TokenType.Plus)),

                // Primitive values, numbers, chars, strings
                If(t => char.IsNumber(t.Peek()) || t.Peek() == '-', new NumberParser()),
                chars,
                strings,

                // If we haven't matched so far, it's an unexected character
                Error<char, PathToken>(t => $"Unexpected character {t.Peek()}")
            );
        }
    }
}