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
        public static IParser<PathToken, IReadOnlyList<IPathStage>> GetParser()
        {
            var singlePath = List(
                First(
                    Token(TokenType.Star, t => (IPathStage) new AllPropertiesNestedPath()),
                    Rule(
                        Token(TokenType.Dot),
                        Token(TokenType.Identifier),
                        (dot, name) => (IPathStage) new NamedPropertyPath(name.Value)
                    ),
                    Transform(Token(TokenType.Dot), t => (IPathStage) new AllPropertiesPath()),
                    Rule(
                        Token(TokenType.OpenBracket),
                        Token(TokenType.CloseBracket),
                        (open, close) => (IPathStage) new AllIndexerItemsPath()
                    ),
                    Rule(
                        Token(TokenType.OpenBracket),
                        SeparatedList(
                            First(
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
                        Token(TokenType.OpenAngle),
                        SeparatedList(
                            Token(TokenType.Identifier),
                            Token(TokenType.Dot),
                            parts => string.Join(".", parts.Select(p => p.Value)),
                            atLeastOne: true
                        ),
                        Token(TokenType.CloseAngle),
                        (open, name, close) => (IPathStage) new TypedPath(name)
                    )
                ),
                // TODO: "{" <Identifer> "=" <value> "}" and other similar predicates
                paths => paths
            );

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