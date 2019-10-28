using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing;
using CSPath.Parsing.Tokenizing;
using CSPath.Paths;

namespace CSPath
{
    public static class ObjectExtensions
    {
        // TODO: Support different path dialects via pluggable grammars/parsers?
        // TODO: Variant to wrap each item in an Item object which will include information about where it came from
        public static IEnumerable<object> Path(this object obj, string path) 
            => Path(obj, null, null, path);

        public static IEnumerable<object> Path(this object obj, IParser<char, PathToken> lexer, IParser<PathToken, IReadOnlyList<IPath>> parser, string path)
        {
            if (obj == null)
                return Enumerable.Empty<object>();
            if (string.IsNullOrEmpty(path))
                return new[] { obj };

            var combinedParser = new PathParser(lexer, parser) as IParser<char, IReadOnlyList<IPath>>;
            var input = new StringCharacterSequence(path);

            var result = combinedParser.Parse(input);
            return result.Success ? result.Value.Filter(new[] { obj }) : null;
        }
    }
}
