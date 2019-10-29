using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSPath.Paths
{
    /// <summary>
    /// Looks for and invokes public indexers on each object with the given index arguments
    /// </summary>
    public class IndexerItemsPath : IPath
    {
        private readonly IReadOnlyList<object> _indices;

        public IndexerItemsPath(IReadOnlyList<object> indices)
        {
            _indices = indices;
        }

        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            return input.SelectMany(GetItems);
        }

        private IEnumerable<object> GetItems(object item)
        {
            var type = item.GetType();
            if (type.IsArray && item is Array array && array.Rank == _indices.Count)
            {
                // TODO: These casts are probably going to fail for mixed-type indices. Need a more robust way to go about it.
                if (_indices.All(i => i is int))
                    return new[] { array.GetValue(_indices.Cast<int>().ToArray()) };
                if (_indices.All(i => i is long))
                    return new[] { array.GetValue(_indices.Cast<long>().ToArray()) };
            }

            // Get public indexers with a single parameter
            // this list should be pretty short, most types will expose at most one or two of these
            var indexer = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.CanRead && p.GetMethod != null && MatchesIndexArguments(p.GetIndexParameters()));
            if (indexer == null)
                return Enumerable.Empty<object>();
            var value = indexer.GetMethod.Invoke(item, _indices.ToArray());
            return new[] { value };
        }

        private bool MatchesIndexArguments(ParameterInfo[] parameters)
        {
            if (parameters == null || parameters.Length != _indices.Count)
                return false;
            return !parameters.Where((t, i) => !t.ParameterType.IsInstanceOfType(_indices[i])).Any();
        }
    }
}