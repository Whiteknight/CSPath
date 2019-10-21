using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing;
using CSPath.Paths;

namespace CSPath
{
    public static class ObjectExtensions
    {
        // TODO: Support different path dialects via pluggable grammars/parsers?
        // TODO: Variant to wrap each item in an Item object which will include information about where it came from

        public static IEnumerable<object> Path(this object obj, string path)
        {
            if (obj == null)
                return Enumerable.Empty<object>();
            if (string.IsNullOrEmpty(path))
                return new[] { obj };
            var parser = new PathParser();
            var pipeline = parser.Parse(path);
            return pipeline.Filter(new[] { obj });
        }
    }
}
