using System.Collections.Generic;
using CSPath.Parsing.Tokenizing;
using CSPath.Paths;

namespace CSPath.Parsing
{
    public class PathParser
    {
        private readonly IParser<PathToken, IReadOnlyList<IPath>> _parser;

        public PathParser(IParser<PathToken, IReadOnlyList<IPath>> parser = null)
        {
            _parser = parser ?? PathGrammar.GetParser();
        }

        public IReadOnlyList<IPath> Parse(string s) => Parse(new StringCharacterSequence(s));

        public IReadOnlyList<IPath> Parse(ISequence<char> chars) => Parse(new Tokenizer(chars));

        public IReadOnlyList<IPath> Parse(Tokenizer t)
        {
            var (success, value) = _parser.Parse(t);
            return !success ? null : value;
        }
    }
}
