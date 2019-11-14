using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public static class PathExtensions
    {
        /// <summary>
        /// Given a pipeline of IPath stages and an input sequence, execute the pipeline on the sequence
        /// and return the result.
        /// </summary>
        /// <param name="stages"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<IValueWrapper> Filter(this IEnumerable<IPath> stages, IEnumerable<IValueWrapper> input)
        {
            if (stages == null || input == null)
                return Enumerable.Empty<IValueWrapper>();
            var current = input;
            foreach (var stage in stages)
                current = stage.Filter(current);
            return current;
        }
    }
}