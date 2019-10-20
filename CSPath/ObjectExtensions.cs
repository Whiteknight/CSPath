using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing;

namespace CSPath
{
    public static class ObjectExtensions
    {
        public static IEnumerable<object> Path(this object obj, string path)
        {
            if (obj == null)
                return Enumerable.Empty<object>();
            if (string.IsNullOrEmpty(path))
                return new[] { obj };
            var parser = new PathParser();
            var pipeline = parser.Parse(path);
            var context = new PathTraverseContext(pipeline);
            return context.Traverse(obj);
        }
    }
}
