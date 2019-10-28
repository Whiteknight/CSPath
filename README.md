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
* `[1,2]` Accesses the (int,int)-keyed indexer of the current object and retrieves element `(1,1)`
* `[1|2]` Accesses the int-keyed indexer of the current object and retrives element `1` and element `2`
* `<TypeName>` Allows only values of the given type (type short name, case-insensitive)
* `<Fully.Qualified.TypeName>` Allows only values of the given type (type full name, case-insensitive)
* `|` Takes all the results from the path on the left and all the results from the path on the right
* `{<path> = <value>}` Returns results which satisfy a predicate

When you include the `CSPath` namespace, the `.Path()` method is available as an extension method on all `object`s. You can invoke it like this:

```csharp
var result = myObj.Path(...);
```

### Examples

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

Any object from a list which has a property "Count" which equals 2

```csharp
var result = myObj.Path("[]{.Count = 2}");
```

## Usages

This package was created to address some needs which arise occasionally:

1. Ability to store a value location from an object graph in a database
    * for example to refer to a particular nested value for an alert
1. Ability to reference a location in an object graph in configuration
    * For example, to setup validations by config instead of hard-coding them
1. Ability to query an object through an API or service boundary

## Design Notes

This package *is not intended for high-performance applications*. There's a limit to how much optimizing we can do when we don't know the types of incoming objects at each stage of the path. The parser performance is decent but not optimal. We are currently focusing on editability and not raw performance.

The parser and tokenizer are both built using combinator-based recursive descent. The parser outputs a list of `IPath` which are then invoked iteratively to return the final result.

XML documents are just markup and don't come with built-in functionality. So, XPath has been designed with a lot of functionality to add some "behavior" to these documents. CSPath doesn't need to do that because C# objects have their own behaviors (methods, etc) and many use-cases can be better served by alternative mechanisms (LINQ Queries, method calls). CSPath does not intend to include all the functionality of XPath, though it does draw inspiration from XPath.