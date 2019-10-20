using System.Collections.Generic;
using System.Linq;

namespace CSPath
{
    public class ObjectRegistry
    {
        private readonly HashSet<object> _seenObjects;

        public ObjectRegistry()
        {
            _seenObjects = new HashSet<object>();
        }

        public void AddReferences(IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                if (!_seenObjects.Contains(obj))
                    _seenObjects.Add(obj);
            }
        }

        public void AddReference(object obj)
        {
            if(!_seenObjects.Contains(obj))
                _seenObjects.Add(obj);
        }

        public bool CanVisit(object obj)
        {
            if (obj == null)
                return true;
            if (obj.GetType().IsValueType)
                return true;
            if (_seenObjects.Contains(obj))
                return false;
            _seenObjects.Add(obj);
            return true;
        }

        public IEnumerable<object> WhereCanVisit(IEnumerable<object> objects) => objects.Where(CanVisit);
    }
}