using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    /// <summary>
    /// Recursively searches the entire object graph and returns all objects contained therein. Returns each
    /// reference type instance only once (though repeated value type instances may be returned multiple times)
    /// </summary>
    public class AllPropertiesNestedPath : IPath
    {
        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            var registry = new ObjectRegistry();

            return input
                .Where(i => i != null && registry.CanVisit(i))
                .SelectMany(i => RecurseSingleObject(i, registry));
        }

        private static IEnumerable<object> RecurseSingleObject(object obj, ObjectRegistry registry)
        {
            var workingSet = new Stack<object>();
            foreach (var prop in obj.GetPublicPropertyValues())
                workingSet.Push(prop);

            while (workingSet.Count > 0)
            {
                var current = workingSet.Pop();
                yield return current;
                var propertyValues = current.GetPublicPropertyValues().Where(registry.CanVisit);
                foreach (var propertyValue in propertyValues)
                    workingSet.Push(propertyValue);
            }
        }

        // ObjectRegistry is currently only used in AllPropertiesNestedPath, so we will scope it here
        // If it becomes more generally usable, we can move it out
        private class ObjectRegistry
        {
            private readonly HashSet<object> _seenObjects;

            public ObjectRegistry()
            {
                _seenObjects = new HashSet<object>();
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
        }
    }
}
