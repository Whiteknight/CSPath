using System;
using System.Collections.Generic;
using CSPath.Parsing.Parsers;

namespace CSPath.Parsing.Tokenizing
{
    public static class TokenizerMethods
    {
        public static IParser<TInput, TOutput> Match<TInput, TOutput>(IEnumerable<TInput> c, Func<TInput[], TOutput> produce)
        {
            return new MatchSequenceParser<TInput, TOutput>(c, produce);
        }
    }
}