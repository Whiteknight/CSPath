using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing;
using CSPath.Parsing.Inputs;
using CSPath.Paths;
using CSPath.Paths.Values;

namespace CSPath
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Starting from the root object, recursively search for all publicly-visible object which match
        /// the given path and return them. Throws exceptions for incorrectly-formatted path strings.
        /// </summary>
        /// <param name="root">The root object from which the search begins. The root object will not be
        /// returned in the output unless there is a circular reference in the object graph</param>
        /// <param name="parser">The parser to use to interpret the path string. If null, the default parser
        /// will be used</param>
        /// <param name="path">The path string. See documentation for syntax and all options.</param>
        /// <returns>An enumeration of all objects from the object graph which match the path.</returns>
        public static IEnumerable<object> Path(this object root, string path, IParser<char, IReadOnlyList<IPath>> parser = null)
        {
            if (root == null)
                return Enumerable.Empty<object>();
            if (string.IsNullOrEmpty(path))
                return new[] { root };

            var input = new StringCharacterSequence(path);

            var result = (parser ?? PathGrammar.DefaultParserInstance).Parse(input);
            if (!result.Success)
                return null;
            return result.Value.Filter(new IValueWrapper[] { new SimpleValueWrapper(root) }).Select(w => w.Value);
        }
    }
}
