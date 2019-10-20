using System.Collections.Generic;
using CSPath.Parsing.Tokenizing;

namespace CSPath.Parsing
{
    public class PathParser
    {
        private readonly IParser<PathToken, IReadOnlyList<IPathStage>> _parser;

        public PathParser(IParser<PathToken, IReadOnlyList<IPathStage>> parser = null)
        {
            _parser = parser ?? PathGrammar.GetParser();
        }

        public IReadOnlyList<IPathStage> Parse(string s) => Parse(new StringCharacterSequence(s));

        public IReadOnlyList<IPathStage> Parse(ISequence<char> chars) => Parse(new Tokenizer(chars));

        public IReadOnlyList<IPathStage> Parse(Tokenizer t)
        {
            var (success, value) = _parser.Parse(t);
            return !success ? null : value;
        }
    }
}
