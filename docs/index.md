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

### Primitive value literals

CSPath supports several types of primitive value literals for use in indexing and predicates (see below for more info on predicates). Here are some examples of literals which are supported:

* Integer literals: `123`, `-123`
* Unsigned, Long and Unsigned Long literals: `123U`, `123L`, `-123L` and `123UL`
* Hex integer literals: `0x123`, `0x123U`, `0x123L`, `0x123UL`
* Float, Double and Decimal literals: `12.34`, `-12.34`, `12.34F`, `-12.34F`, `12.34M` and `-12.34M`
* Character literals: `'a'`, `'\t'`, `'\x12AB'`, `'\u12AB'`, `'\U00010FFF'`
* String literals: `"test"`, `"\tes\t"`, `"test\x12AB"`, `"test\u12AB"`, `"test\U00010FFF"`
* Boolean literals: `true`, `false`
* Null: `null`

## Type Constraints

You can filter values by type with the `< >` operators:

```csharp
var allListsOfString = myObjects.Path("<List<string>>");

var allListsOfString = myObjects.Path("<Generic.List<String>>");

var allListsOfString = myObjects.Path("<System.Collections.Generic.List<System.String>>");
```

This type-matching feature can use type short-names or fully-qualified names, or things in between. Notice that these strings don't respect any `using` directives you have active in your C# code file, those directives disappear from the CLR IL after compilation. The three examples above will all match a `System.Collections.Generic.List<string>` instance, but the first two may also match other classes which are also named `List` but live in other namespaces. The type matching algorithm is a suffix-matching algorithm, so it will start from the right and try to match as much as it has. This may lead to situations you didn't expect where classes have the same names.

## Predicates

You can filter by basic predicates using the `{ }` operators and several regex-inspired arity modifiers. Here are a couple forms of predicate:

```csharp
// Any object from the list which has exactly 2 properties of type string
var has2Strings = myList.Path("[]{ .<string> }{2}");

// From a list-of-lists, get all lists with 3 or 4 items
var listsWith3Or4Items = myList.Path("[] { [] }{3,4}");

// The Price from any LineItem which is Taxable
var taxablePrices = myOrder.Path(".LineItems[]{ .IsTaxable = true }*.Price");
```

Paths must be on the left, followed optionally by a comparison operator and a primitive value. The following comparison operators are supported:

* `=` and `==` Both denote equality (the former is included as a shorthand)
* `!=` Denotes inequality

Comparison values can be any of the primitive values described under the "Indexers" heading, above.

Arity modifiers are:

* `*` All objects match, or an empty list
* `+` All objects match, at least one
* `{N}` Exactly N objects match (where N is an integer)
* `{N,}` N or more objects match (where N is an integer)
* `{,N}` At most N objects match (where N is an integer)
* `{N,M}` At least N but at most M objects match (where N and M are integers)

## Combining Paths

You can string together as many path operators as you want to find the data you are looking for:

```csharp
var allPrices = myOrder.Path(".LineItems*.Price");

var allTaxablePrices = myOrder.Path(".LineItems[]{.IsTaxable = true}.Price");

var allServiceCharges = myOrder.Path(".LineItems[]<ServiceCharge>");
```

You can also group alternations or any other path with parenthesis `( )`:

```csharp
var grouped = myObj.Path("(.Value1 | .Value2).Child");
```

Finally it's worth mentioning that you can organize your paths a little more nicely by using whitespace to separate different bits:

```csharp
var allTaxablePrices = myOrder.Path(@"
    .LineItems
    []
    { .IsTaxable = true }
    .Price"
);
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
