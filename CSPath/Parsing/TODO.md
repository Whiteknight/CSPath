# Parsing TODO

## Improve predicate syntax

### Option 1 "functions"

```
{all(.path = value)}
{any(.path)}
{any(.path = value)}
{none(.path)}
{none(.path = value)}
{exactly(1, .path)}
{exactly(1, .path = value)}
{atleast(1, .path)}
{atleast(1, .path = value)}
{atmost(1, .path)}
{atmost(1, .path = value)}
```

TODO: Need a syntax to differentiate "all, if any" or "all, at least one"

### Option 2 postfix

```
{.path = value}*
{.path = value}*?
{.path = value}+
{.path = value}?
```

TODO: Need to flesh out the arity methods

### Option 3 regex-like

```
(.path = value)*
(.path = value)?
(.path = value)+
(.path = value){1}
(.path = value){1,}
(.path = value){,2}
(.path = value){1,4}
```

## Type Constraints

### Handle generic types

```
<List<int>>
```

### Handle array types

```
<int[]>
```

### TypedPath needs to property handle nested child types


