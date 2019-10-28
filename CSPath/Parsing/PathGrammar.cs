using System;
using System.Collections.Generic;
using System.Linq;
using CSPath.Parsing.Tokenizing;
using CSPath.Paths;
using static CSPath.Parsing.Parsers.ParserMethods;
using static CSPath.Parsing.PathParserMethods;

namespace CSPath.Parsing
{
    public static class PathGrammar
    {
        private static readonly Lazy<IParser<PathToken, IReadOnlyList<IPath>>> _instance = new Lazy<IParser<PathToken, IReadOnlyList<IPath>>>(GetParser);
        public static IParser<PathToken, IReadOnlyList<IPath>> DefaultInstance => _instance.Value;

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
                Token(TokenType.False, t => (object)false),
                Token(TokenType.Null, t => (object)null)
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
                    // TODO: semi-colon separated lists
                    SeparatedList(
                        primitiveValues,
                        Token(TokenType.Comma),
                        indices => indices
                    ),
                    Token(TokenType.CloseBracket), 
                    (open, indices, close) => indices.Count == 0 ? (IPath)new AllIndexerItemsPath() : new IndexerItemsPath(indices)
                )
            );

            // typeConstraint = "<" <identifier> ("." <identifier>)* ">"
            var typeConstraintPaths = Rule(
                Token(TokenType.OpenAngle),
                // TODO: Generic and array types
                Require(
                    SeparatedList(
                        Token(TokenType.Identifier),
                        Token(TokenType.Dot),
                        parts => string.Join(".", parts.Select(p => p.Value)),
                        atLeastOne: true
                    ),
                    t => $"Unexpected token in type constraint {t.Peek()}"
                ),
                Require(
                    Token(TokenType.CloseAngle),
                    t => $"Expected '>' but found {t.Peek()}"
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
                Require(
                    Deferred(() => singlePath),
                    t => $"Expected path but found {t.Peek()}"
                ),
                First(
                    // TODO: Support other equality comparisons < > <= >= !=
                    Token(TokenType.Equals),
                    Error<PathToken, PathToken>(t => $"Expected '=' but found {t.Peek()}")
                ),
                First(
                    Token(TokenType.Star, t => t.Value),
                    Token(TokenType.Plus, t => t.Value),
                    Token(TokenType.Bar, t => t.Value),
                    Produce<PathToken, string>(() => "")
                ),
                First(
                    primitiveValues,
                    Error<PathToken, object>(t => $"Expected primitive value but found {t.Peek()}")
                ),
                Require(
                    Token(TokenType.CloseBrace),
                    t => $"Expected '}}' but found {t.Peek()}"
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
                Require(
                    Token(TokenType.EndOfInput),
                    t => $"Expected EndOfInput but found {t.Peek()}"
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