using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CSPath.Paths
{
    public class IndexerItemsPath : IPathStage
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
            if (type.IsArray)
            {
                var array = item as Array;
                return _indices.Where(i => i is int).Select(i => array.GetValue((int) i));
            }

            var a1 = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetMethod != null)
                .ToList()
                ;

            var a2 = a1.Select(p => new
            {
                Property = p,
                IndexParameters = p.GetIndexParameters()
            })
            .Where(x => x.IndexParameters != null && x.IndexParameters.Length == 1)
            .ToList();

            var a3 = a2.Select(x => new
            {
                Property = x.Property,
                ParameterType = x.IndexParameters.Single().ParameterType,
            })
                .ToList();
            var indexers = a3.ToList();
            var results = new List<object>();
            foreach (var index in _indices)
            {
                var indexer = indexers.FirstOrDefault(idxr => idxr.ParameterType.IsInstanceOfType(index));
                if (indexer == null)
                    continue;
                var value = indexer.Property.GetMethod.Invoke(item, new[] { index });
                results.Add(value);
            }
            return results;
        }
    }
}