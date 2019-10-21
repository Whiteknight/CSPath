using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing.Parsers;
using CSPath.Parsing.Tokenizing;
using CSPath.Paths;
using static CSPath.Parsing.Parsers.ParserMethods;
using static CSPath.Parsing.PathParserMethods;

namespace CSPath.Parsing
{
    public static class PathGrammar
    {
        // TODO: Static instance of IParser so we only need to build it once

        public static IParser<PathToken, IReadOnlyList<IPath>> GetParser()
        {
            var primitiveValues = First(
                Token(TokenType.Integer, t => (object)int.Parse(t.Value)),
                Token(TokenType.String, t => (object)t.Value),
                Token(TokenType.Character, t => (object)t.Value[0]),
                Token(TokenType.Double, t => (object)int.Parse(t.Value)),
                Token(TokenType.Long, t => (object)int.Parse(t.Value)),
                Token(TokenType.Float, t => (object)int.Parse(t.Value)),
                Token(TokenType.Decimal, t => (object)int.Parse(t.Value)),
                Token(TokenType.UInteger, t => (object)int.Parse(t.Value)),
                Token(TokenType.ULong, t => (object)int.Parse(t.Value)),
                Token(TokenType.True, t => (object)true),
                Token(TokenType.False, t => (object)false)
            );

            IParser<PathToken, IReadOnlyList<IPath>> singlePath = null;

            // property = "*" | "." <identifier> | "."
            var propertyPaths = First(
                Token(TokenType.Star, t => (IPath) new AllPropertiesNestedPath()),
                Rule(
                    Token(TokenType.Dot),
                    Token(TokenType.Identifier),
                    (dot, name) => (IPath) new NamedPropertyPath(name.Value)
                ),
                Transform(Token(TokenType.Dot), t => (IPath) new AllPropertiesPath())
            );

            // indexer = "[" "]" | "[" (<integer> | <string>)+ "]"
            var indexerPaths = First(
                Rule(
                    Token(TokenType.OpenBracket),
                    Token(TokenType.CloseBracket),
                    (open, close) => (IPath) new AllIndexerItemsPath()
                ),
                Rule(
                    Token(TokenType.OpenBracket),
                    SeparatedList(
                        First(
                            primitiveValues,
                            new ThrowExceptionParser<PathToken, object>(t => $"Unexpected token in indexer {t.Peek()}")
                        ),
                        Token(TokenType.Comma),
                        indices => indices,
                        atLeastOne: true
                    ),
                    Token(TokenType.CloseBracket),
                    (open, indices, close) => (IPath) new IndexerItemsPath(indices)
                ),
                Rule(
                    Token(TokenType.OpenBracket),
                    new ThrowExceptionParser<PathToken, object>(t => $"Expected ']' or key, found {t.Peek()}"),
                    (open, err) => (IPath) null
                )
            );

            // typeConstraint = "<" <identifier> ("." <identifier>)* ">"
            var typeConstraintPaths = Rule(
                Token(TokenType.OpenAngle),
                // TODO: Generic and array types
                First(
                    SeparatedList(
                        Token(TokenType.Identifier),
                        Token(TokenType.Dot),
                        parts => string.Join(".", parts.Select(p => p.Value)),
                        atLeastOne: true
                    ),
                    new ThrowExceptionParser<PathToken, string>(t => $"Unexpected token in type constraint {t.Peek()}")
                ),
                First(
                    Token(TokenType.CloseAngle),
                    new ThrowExceptionParser<PathToken, PathToken>(t => $"Expected '>' but found {t.Peek()}")
                ),
                (open, name, close) => (IPath) new TypedPath(name)
            );

            // TODO: Form where we test that the selector returns at least some items without comparing values
            // TODO: Form where we test that the selector returns no values, without comparisons
            // TODO: Form where we test that the selector returns exactly one value, without comparison
            // TODO: We could acheive this with named methods atleast(n), none() or exactly(n)
            // "{" (<property> | <indexer>) "=" ("*" | "&" | "|" | "") (<primitive> | "null")
            var predicateComparePaths = Rule(
                Token(TokenType.OpenBrace),
                First(
                    Deferred(() => singlePath),
                    new ThrowExceptionParser<PathToken, IReadOnlyList<IPath>>(t => $"Expected path but found {t.Peek()}")
                ),
                First(
                    // TODO: Support other equality comparisons < > <= >= !=
                    Token(TokenType.Equals),
                    new ThrowExceptionParser<PathToken, PathToken>(t => $"Expected '=' but found {t.Peek()}")
                ),
                First(
                    Token(TokenType.Star, t => t.Value),
                    Token(TokenType.Plus, t => t.Value),
                    Token(TokenType.Bar, t => t.Value),
                    Produce<PathToken, string>(() => "")
                ),
                First(
                    primitiveValues,
                    Token(TokenType.Null, t => (object)null),
                    new ThrowExceptionParser<PathToken, object>(t => $"Expected primitive value but found {t.Peek()}")
                ),
                First(
                    Token(TokenType.CloseBrace),
                    new ThrowExceptionParser<PathToken, PathToken>(t => $"Expected '}}' but found {t.Peek()}")
                ),
                (open, path, eq, mod, value, close) => (IPath)new PredicatePath(path, eq.Value, value, mod)
            );

            // single = (<property> | <indexer> | <typeConstraint>)*
            singlePath = List(
                First(
                    propertyPaths,
                    indexerPaths,
                    typeConstraintPaths,
                    predicateComparePaths
                ),
                paths => paths
            );

            // concat = <single> ("|" <single>)* 
            var concatPaths = SeparatedList(
                singlePath,
                Token(TokenType.Bar),
                paths => paths.Count == 1 ? paths[0] : new IPath[] { new CombinePath(paths) }
            );

            // <concat> <EndOfInput>
            var all = Rule(
                concatPaths,
                First(
                    Token(TokenType.EndOfInput),
                    new ThrowExceptionParser<PathToken, PathToken>(t => $"Expected EndOfInput but found {t.Peek()}")
                ),
                (stages, eoi) => stages
            );

            return all;

            //return List(
            //    First(
            //        member
            //    ),
            //    stages => 
            //        (IPathStage) new PipelinePath(stages)
            //);
        }
    }
}