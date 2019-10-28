using System.Collections.Generic;
using CSPath.Parsing.Tokenizing;
using CSPath.Paths;

namespace CSPath.Parsing
{
    public class PathParser : IParser<char, IReadOnlyList<IPath>>, IParser<PathToken, IReadOnlyList<IPath>>
    {
        private readonly IParser<char, PathToken> _lexer;
        private readonly IParser<PathToken, IReadOnlyList<IPath>> _parser;

        public PathParser()
            : this(null, null)
        {
        }

        public PathParser(IParser<char, PathToken> lexer, IParser<PathToken, IReadOnlyList<IPath>> parser)
        {
            _lexer = lexer ?? LexicalGrammar.DefaultInstance;
            _parser = parser ?? PathGrammar.DefaultInstance;
        }

        IParseResult<object> IParser<char>.ParseUntyped(ISequence<char> t) 
            => _parser.Parse(new Tokenizer(t, _lexer));

        IParseResult<IReadOnlyList<IPath>> IParser<char, IReadOnlyList<IPath>>.Parse(ISequence<char> t) 
            => _parser.Parse(new Tokenizer(t, _lexer));

        IParseResult<object> IParser<PathToken>.ParseUntyped(ISequence<PathToken> t) 
            => _parser.Parse(t);

        IParseResult<IReadOnlyList<IPath>> IParser<PathToken, IReadOnlyList<IPath>>.Parse(ISequence<PathToken> t) 
            => _parser.Parse(t);
    }
}
