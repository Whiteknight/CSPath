using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CSPath.Paths
{
    public class AllPropertiesNestedPath : IPathStage
    {
        public IEnumerable<object> Filter(IEnumerable<object> input)
        {
            var registry = new ObjectRegistry();
            var workingSet = new Stack<object>();
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
    }
}
