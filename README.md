# CSPath
XPath-like accessor for values in arbitrary object graphs

## Paths

CSPath works like XPath, where you write a path as a string, and CSPath will return the corresponding objects from the object graph. However, it attempts to use a syntax more familiar to C# developers:

* `.` Get all property values of the current object
* `.PropertyName` Get the value of property `PropertyName`
* `*` Get all property values of the current object, recursively (will avoid circular references)
* `[]` Get all elements (treats the current object as `IEnumerable` and returns all results)
* `[0]` Accesses the int-keyed indexer of the current object and retrieves element `0`
* `["key"]` Accesses the string-keyed indexer of the current object and retrieves element `"key"`
* `<TypeName>` Allows only values of the given type (type short name, case-insensitive)
* `<Fully.Qualified.TypeName>` Allows only values of the given type (type full name, case-insensitive)

When you include the `CSPath` namespace, the `.Path()` method is available as an extension method on all `object`s. You can invoke it like this:

```csharp
var result = myObj.Path(...);
```

## Examples

Get all property values of the current object:

```csharp
var result = myObj.Path(".");
```

Get all property values of type `Int32` from the current object:

```csharp
var result = myObj.Path(".<Int32>");
```

Get all values of the list:

```csharp
var result = myObj.Path("[]");
```

Get the first value of the list:

```csharp
var result = myObj.Path("[0]");
```

Navigate a deeply-nested structure

```csharp
var result = myObj.Path(".Items[0].Values[\"Key\"].Length");
```

## Design Notes

This package *is not intended for high-performance applications*. There's a limit to how much optimizing we can do when we don't know the types of incoming objects at each stage of the path.

