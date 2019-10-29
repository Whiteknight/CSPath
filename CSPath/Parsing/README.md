# CSPath Parser

Here are a few notes about the parser for anybody interested in reading it or even contributing to it.

1. The parser is currently *scannerless*, meaning there aren't separately defined lexical analysis and parsing phases. Instead there's a single combined parser which takes character inputs and produces the necessary `IPath` outputs. We started with a 2-phase lexer/parser combo, but this decreased performance without giving a large benefit in readability or maintainability.
2. The parser is *recursive descent* using combinator objects. This allows us to define the structure of the grammar more clearly than normal recursive-descent. The final parser is an object graph of individual parser objects. Using static methods to simplify graph creation makes a readable grammar.
3. 