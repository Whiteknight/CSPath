# CSPath
XPath-like accessor for values in arbitrary object graphs

## Paths

CSPath works like XPath, where you write a path as a string, and CSPath will return the corresponding objects from the object graph. However, it attempts to use a syntax more familiar to C# developers:

* `.` Get all property values of the current object
* `.PropertyName` Get the value of property `PropertyName`
* `*` Get all property values of the current object, recursively
* `[]` Get all elements (treats the current object as `IEnumerable` and returns all results)
* `[0]` Accesses the int-keyed indexer of the current object and retrieves element `0`
* `["key"]` Accesses the string-keyed indexer of the current object and retrieves element `"key"`
