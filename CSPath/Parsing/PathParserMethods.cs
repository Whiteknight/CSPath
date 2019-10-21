using System;
using System.Linq;
using CSPath.Parsing.Parsers;
using CSPath.Parsing.Tokenizing;

namespace CSPath.Parsing
{
    public static class PathParserMethods
    {
        public static IParser<PathToken, PathToken> Token(TokenType type)
        {
            return new PredicateParser<PathToken, PathToken>(t => t.IsType(type), p => p);
        }

        public static IParser<PathToken, TOutput> Token<TOutput>(TokenType type, Func<PathToken, TOutput> produce)
        {
            return new PredicateParser<PathToken, TOutput>(t => t.IsType(type), produce);
        }
    }
}