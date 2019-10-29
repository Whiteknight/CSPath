using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public static class PathStageExtensions
    {
        /// <summary>
        /// Given a pipeline of IPath stages and an input sequence, execute the pipeline on the sequence
        /// and return the result.
        /// </summary>
        /// <param name="stages"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<object> Filter(this IEnumerable<IPath> stages, IEnumerable<object> input)
        {
            if (stages == null || input == null)
                return Enumerable.Empty<object>();
            var current = input;
            foreach (var stage in stages)
                current = stage.Filter(current);
            return current;
        }
    }
}