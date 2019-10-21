using System.Collections.Generic;
using System.Linq;

namespace CSPath.Paths
{
    public class AllPropertiesNestedPath : IPath
    {
        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            var registry = new ObjectRegistry();
            var workingSet = new Stack<object>();
            // input is arbitrarily large, so we don't want to push it all onto the workingSet
            // plus pushing them all in would reverse the order. order isn't required, but it's still unnecessary
            foreach (var obj in input)
            {
                if (obj == null)
                    continue;
                if (!registry.CanVisit(obj))
                    continue;
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
