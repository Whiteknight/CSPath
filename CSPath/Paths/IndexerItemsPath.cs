using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSPath.Paths
{
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
            // TODO: Need to change this so _indices are arguments to a single indexer, instead of consecutive calls to multiple indexers
            var type = item.GetType();
            if (type.IsArray && item is Array array)
                return _indices.Where(i => i is int).Select(i => array.GetValue((int) i));

            // Get public indexers with a single parameter
            // this list should be pretty short, most types will expose at most one or two of these
            var indexers = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetMethod != null)
                .Select(p => new
                {
                    Property = p,
                    IndexParameters = p.GetIndexParameters()
                })
                .Where(x => x.IndexParameters != null && x.IndexParameters.Length == 1)
                .Select(x => new
                {
                    Property = x.Property,
                    ParameterType = x.IndexParameters.Single().ParameterType,
                })
                .ToList();

            // foreach index, see if we have a matching indexer. If so, get the value
            return _indices
                .Select(idx => new
                {
                    Index = idx,
                    Indexer = indexers.FirstOrDefault(idxr => idxr.ParameterType.IsInstanceOfType(idx))
                })
                .Where(x => x.Indexer != null)
                .Select(x => x.Indexer.Property.GetMethod.Invoke(item, new[] { x.Index }));
        }
    }
}