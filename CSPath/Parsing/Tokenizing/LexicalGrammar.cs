using System.Collections.Generic;
using System.Linq;
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
            var numbers = BuildNumberParser();

            // identifier = (<char> | "_") (<char> | <digit> | "_")*
            var identifiers = Rule(
                Match<char>(c => char.IsLetter(c) || c == '_'),
                List(
                    Match<char>(c => char.IsLetter(c) || char.IsDigit(c) || c == '_'),
                    c => c.ToArray()
                ),
                (start, rest) => new PathToken(start.ToString() + new string(rest), TokenType.Identifier)
            );

            var chars = BuildCharacterLiteralParser();

            var strings = BuildStringParser();

            var allItems = First(
                // input char sequence returns "\0" for end-of-input. Detect that and return an EOI token
                Match("\0", c => PathToken.EndOfInput()),

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
                numbers,
                chars,
                strings,

                // If we haven't matched so far, it's an unexected character
                Error<char, PathToken>(t => $"Unexpected character {t.Peek()}")
            );

            return Rule(
                List(Match<char>(char.IsWhiteSpace), t => (object)null),
                allItems,
                (ws, item) => item
            );
        }

        private static IParser<char, PathToken> BuildStringParser()
        {
            var hexCharLiterals = Rule(
                Match("\\", c => c[0]),
                Match<char>(c => c == 'x'),
                List(
                    Match<char>(c => _hexDigits.Contains(c)),
                    t => (char) int.Parse(new string(t.ToArray()))
                ),
                (slash, escape, c) => c.ToString()
            );
            var utf16CharLiterals = Rule(
                Match("\\", c => c[0]),
                Match<char>(c => c == 'u'),
                List(
                    // TODO: 1-4 of these or exactly-4?
                    Match<char>(c => _hexDigits.Contains(c)),
                    t => char.ConvertFromUtf32(int.Parse(new string(t.ToArray())))
                ),
                (slash, escape, c) => c
            );
            var utf32CharLiterals = Rule(
                Match("\\", c => c[0]),
                Match<char>(c => c == 'U'),
                List(
                    // TODO: 1-8 of these or exactly 8?
                    Match<char>(c => _hexDigits.Contains(c)),
                    t => char.ConvertFromUtf32(int.Parse(new string(t.ToArray())))
                ),
                (slash, escape, c) => c
            );
            var escapeChars = Rule(
                Match("\\", c => c[0]),
                Match<char, string>(c => "abfnrtv\\'\"0".Contains(c), c => c.ToString()),
                (slash, c) => c
            );

            // strings = """ (<hexSequence> | <utf16Char> | <utf32Char> | <escapedChar> | .)* """
            var strings = Rule(
                Match("\"", c => c[0]),
                List(
                    First(
                        hexCharLiterals,
                        utf16CharLiterals,
                        utf32CharLiterals,
                        escapeChars,
                        Match<char, string>(c => c != '\0' && c != '"', c => c.ToString())
                    ),
                    s => string.Join("", s)
                ),
                First(
                    Match("\"", c => c[0]),
                    Error<char, char>(t => $"Expected end doublequote but found {t.Peek()}")
                ),
                (start, body, end) => new PathToken(body, TokenType.String)
            );
            return strings;
        }

        private static IParser<char, PathToken> BuildCharacterLiteralParser()
        {
            var hexCharLiterals = Rule(
                Match<char>(c => c == '\\'),
                Match<char>(c => c == 'x'),
                List(
                    // TODO: 1-4 of these only
                    Match<char>(c => _hexDigits.Contains(c)),
                    t => (char) int.Parse(new string(t.ToArray()))
                ),
                (slash, x, c) => c
            );
            var unicodeCharLiterals = Rule(
                Match<char>(c => c == '\\'),
                Match<char>(c => c == 'u'),
                List(
                    // TODO: 1-4 of these or exactly-4 of these?
                    Match<char>(c => _hexDigits.Contains(c)),
                    t => char.ConvertFromUtf32(int.Parse(new string(t.ToArray())))[0]
                ),
                (slash, u, c) => c
            );
            var escapeChars = Rule(
                Match<char>(c => c == '\\'),
                Match<char>(c => "abfnrtv\\'\"0".Contains(c)),
                (slash, c) => c
            );

            // chars = "'" ("\" "x" <hexdigit>+ | "\" . | .) "'"
            var chars = Rule(
                Match<char>(c => c == '\''),
                First(
                    hexCharLiterals,
                    unicodeCharLiterals,
                    escapeChars,
                    Match<char>(c => c != '\0' && c != '\''),
                    Error<char, char>(t => $"Expected char value but found {t.Peek()}")
                ),
                First(
                    Match<char>(c => c == '\''),
                    Error<char, char>(t => $"Expected end singlequote but found {t.Peek()}")
                ),
                (start, content, end) => new PathToken(content.ToString(), TokenType.Character)
            );
            return chars;
        }

        private static IParser<char, PathToken> BuildNumberParser()
        {
            // TODO: "_" separator in a number
            // "0x" <hexDigit>+ | "-"? <digit>+ "." <digit>+ <type>? | "-"? <digit>+ <type>?
            var hexLiterals = Rule(
                Match("0x", c => c),
                List(
                    Match<char>(c => _hexDigits.Contains(c)),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                First(
                    Match("UL", c => TokenType.ULong),
                    Match("U", c => TokenType.UInteger),
                    Match("L", c => TokenType.Long),
                    Produce<char, TokenType>(() => TokenType.Integer)
                ),
                (prefix, body, type) => new PathToken(int.Parse(body, System.Globalization.NumberStyles.HexNumber).ToString(), type)
            );
            var dottedNumbers = Rule(
                Optional(Match("-", c => "-"), () => ""),
                List(
                    Match<char>(char.IsDigit),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                Match(".", c => "."),
                List(
                    Match<char>(char.IsDigit),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                First(
                    Match("F", c => TokenType.Float),
                    Match("M", c => TokenType.Decimal),
                    Produce<char, TokenType>(() => TokenType.Double)
                ),
                (neg, whole, dot, fract, type) => new PathToken(neg + whole + "." + fract, type)
            );
            var integralNumbers = Rule(
                Optional(Match("-", c => "-"), () => ""),
                List(
                    Match<char>(char.IsDigit),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                First(
                    Match("UL", c => TokenType.ULong),
                    Match("U", c => TokenType.UInteger),
                    Match("L", c => TokenType.Long),
                    Produce<char, TokenType>(() => TokenType.Integer)
                ),
                (neg, whole, type) => new PathToken(neg + whole, type)
            );

            var numbers = First(
                hexLiterals,
                dottedNumbers,
                integralNumbers
            );
            return numbers;
        }
    }
}