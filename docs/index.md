CSPath is an XPath-inspired object traversal library using syntax familiar to C# developers.

The most straight-forward way to work with CSPath is to include the namespace and call the `.Path()` extension method:

```csharp
using CSPath;

var result = myObj.Path("");
```

## Properties Paths

The `.` operator allows you to access values of public properties.

* `.` A dot by itself enumerates the values of all public properties
* `.Name` A dot with a name gets the value of the public property with the same name
* `*` An asterisk searches all properties in the object graph recursively, returning all public property values (ignoring circular references).

```csharp
var allProperties = myObj.Path(".");

var justLength = myObj.Path(".Length");
```

## Alternations

The `|` operator combines the results from two or more paths:

```csharp
var allMoneyValues = myObj.Path(".Cost | .Price");
```

## Indexers

The `[ ]` operators denote an indexing operation

* `[]` Empty brackets treat the current object as an `IEnumerable` and enumerate it.
* `[1]` or `["test"]` or `[1, 2]` You can use most primitive values as arguments to the indexer with expected results
* `[1 | 2]` The `|` operator combines results. This is the same as `[1] | [2]`.

## Type Constraints

You can filter values by type with the `< >` operators:

```csharp
var allListsOfString = myObjects.Path("<List<string>>");

var allListsOfString = myObjects.Path("<Generic.List<String>>");

var allListsOfString = myObjects.Path("<System.Collections.Generic.List<System.String>>");
```

This type-matching feature can use type short-names or fully-qualified names, or things in between. Notice that these strings don't respect any `using` directives you have active in your C# code file, those directives disappear from the CLR IL after compilation. The three examples above will all match a `System.Collections.Generic.List<string>` instance, but the first two may also match other classes which are also named `List` but live in other namespaces. The type matching algorithm is a suffix-matching algorithm, so it will start from the right and try to match as much as it has. This may lead to situations you didn't expect where classes have the same names.

## Predicates

You can filter by basic predicates using the `{ }` operators.

TBD

## Combining Paths

You can string together as many path operators as you want to find the data you are looking for:

```csharp
var allPrices = myOrder.Path(".LineItems*.Price");

var allTaxablePrices = myOrder.Path(".LineItems[]{.IsTaxable = true}.Price");

var allServiceCharges = myOrder.Path(".LineItems[]<ServiceCharge>");
```

## Warnings

This library is intended for rare situations, and most cases of object graph traversal would be better served by LINQ method chains. Some of the problems to consider are:

* Performance of CSPath paths will be drastically decreased compared to LINQ expressions (runtime parsing effort plus reflection overhead).
* CSPath paths are not checked at compile-type. Typos and errors will not be found until runtime.
* CSPath paths are not resilient against refactoring. Changes in your object structure will break existing CSPath path strings.

These issues not withstanding, there are some (rare) occasions when CSPath might still help your project. See if most of these conditions are true:

* Your object structure doesn't change much, or you use semantic versioning and your CSPath path strings can be tied to major version numbers
* Your use-case is dynamic in nature and cannot be satisfied by normal LINQ methods or other object-traversal techniques.
* Your path strings are not known at compile time

If your use-case meets all these requirements, and you understand the problems with using this tool, CSPath can be a helpful and handy little tool to have around.
