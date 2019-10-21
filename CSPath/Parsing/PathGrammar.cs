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

        public static IParser<PathToken, IReadOnlyList<IPathStage>> GetParser()
        {
            // property = "*" | "." <identifier> | "."
            var propertyPaths = First(
                Token(TokenType.Star, t => (IPathStage) new AllPropertiesNestedPath()),
                Rule(
                    Token(TokenType.Dot),
                    Token(TokenType.Identifier),
                    (dot, name) => (IPathStage) new NamedPropertyPath(name.Value)
                ),
                Transform(Token(TokenType.Dot), t => (IPathStage) new AllPropertiesPath())
            );

            // indexer = "[" "]" | "[" (<integer> | <string>)+ "]"
            var indexerPaths = First(
                Rule(
                    Token(TokenType.OpenBracket),
                    Token(TokenType.CloseBracket),
                    (open, close) => (IPathStage) new AllIndexerItemsPath()
                ),
                Rule(
                    Token(TokenType.OpenBracket),
                    SeparatedList(
                        First(
                            // TODO: More key types. Should be able to support all primitive values
                            Token(TokenType.Integer, t => (object) int.Parse(t.Value)),
                            Token(TokenType.String, t => (object) t.Value),
                            new ThrowExceptionParser<PathToken, object>(t => $"Unexpected token in indexer {t.Peek()}")
                        ),
                        Token(TokenType.Comma),
                        indices => indices,
                        atLeastOne: true
                    ),
                    Token(TokenType.CloseBracket),
                    (open, indices, close) => (IPathStage) new IndexerItemsPath(indices)
                ),
                Rule(
                    Token(TokenType.OpenBracket),
                    new ThrowExceptionParser<PathToken, object>(t => $"Expected ']' or key, found {t.Peek()}"),
                    (open, err) => (IPathStage)null
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
                (open, name, close) => (IPathStage) new TypedPath(name)
            );

            // single = (<property> | <indexer> | <typeConstraint>)*
            var singlePath = List(
                First(
                    propertyPaths,
                    indexerPaths,
                    typeConstraintPaths
                ),
                // TODO: "{" <Path> "=" <value> "}" and other similar predicates
                paths => paths
            );

            // concat = <single> ("|" <single>)*
            var concatPaths = SeparatedList(
                singlePath,
                Token(TokenType.Bar),
                paths => paths.Count == 1 ? paths[0] : new IPathStage[] { new CombinePath(paths) }
            );

            return concatPaths;

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