using System;
using System.Collections.Generic;
using System.Linq;
using CSPath.Paths;
using CSPath.Predicates;
using CSPath.Types;
using static CSPath.Parsing.Parsers.ParserMethods;

namespace CSPath.Parsing
{
    public class PathGrammar
    {
        private static readonly HashSet<char> _hexDigits = new HashSet<char>("abcdefABCDEF0123456789");
        private static readonly HashSet<char> _escapeChars = new HashSet<char>("abfnrtv\\'\"0");
        private static readonly Lazy<IParser<char, IReadOnlyList<IPath>>> _instance = new Lazy<IParser<char, IReadOnlyList<IPath>>>(() => new PathGrammar().GetParser());

        private readonly IParser<char, string> _identifiers;
        private readonly IParser<char, object> _primitives;
        private readonly IParser<char, IReadOnlyList<IPath>> _singlePathInternal;
        private readonly IParser<char, IReadOnlyList<IPath>> _singlePath;
        private readonly IParser<char, IReadOnlyList<IPath>> _concatPaths;
        private readonly IParser<char, object> _whitespace;
        private readonly IParser<char, IReadOnlyList<IPath>> _parser;

        public PathGrammar()
        {
            // singlePath is used recursively, so set up a reference
            _singlePath = Deferred(() => _singlePathInternal);
            _whitespace = List(Match<char>(char.IsWhiteSpace), c => c);
            _identifiers = InitializeIdentifiersParser();
            _primitives = InitializePrimitivesParser();
            _singlePathInternal = InitializeSinglePathsParser();

            // concat = <single> ("|" <single>)* 
            _concatPaths = SeparatedList(
                _singlePathInternal,
                LeadingWhitespace(Characters("|")),
                paths => paths.Count == 1 ? paths[0] : new IPath[] { new CombinePath(paths) }
            );

            // <concat> <EndOfInput>
            _parser = Rule(
                _concatPaths,
                LeadingWhitespace(RequireCharacters("\0", "end of input")),
                (stages, eoi) => stages
            );
        }

        public static IParser<char, IReadOnlyList<IPath>> DefaultParserInstance => _instance.Value;

        public IParser<char, IReadOnlyList<IPath>> GetParser() => _parser;

        private IParser<char, IReadOnlyList<IPath>> InitializeSinglePathsParser()
        {
            var propertyPaths = InitializePropertiesParser();
            var indexerPaths = InitializeIndexersParser();
            var typeConstraintPaths = InitializeTypeConstraintsParser();
            var predicatePaths = InitializePredicatesParser();
            var groupedPaths = Rule(
                Characters("("),
                Deferred(() => _concatPaths),
                Characters(")"),
                (open, path, close) => path.Count == 1 ? path[0] : new GroupedPath(path)
            );

            // single = (<property> | <indexer> | <typeConstraint>)*
            return List(
                LeadingWhitespace(
                    First(
                        propertyPaths,
                        indexerPaths,
                        typeConstraintPaths,
                        predicatePaths,
                        groupedPaths
                    )
                ),
                paths => paths
            );
        }

        private static IParser<char, string> InitializeIdentifiersParser()
        {
            var idStartCharacters = Match<char>(c => char.IsLetter(c) || c == '_');
            var idBodyCharacters = Match<char>(c => char.IsLetter(c) || char.IsDigit(c) || c == '_');

            // identifier = (<char> | "_") (<char> | <digit> | "_")*
            return Rule(
                idStartCharacters,
                List(
                    idBodyCharacters,
                    c => c.ToArray()
                ),
                (start, rest) => start + new string(rest)
            );
        }

        private static IParser<char, object> InitializePrimitivesParser()
        {
            var hexCharacters = Match<char>(c => _hexDigits.Contains(c));

            // TODO: "_" separator in a number
            // "0x" <hexDigit>+ | "-"? <digit>+ "." <digit>+ <type>? | "-"? <digit>+ <type>?
            var hexNumbers = Rule(
                Characters("0x"),
                List(
                    hexCharacters,
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                First(
                    Characters("UL"),
                    Characters("U"),
                    Characters("L"),
                    Produce<char, string>(() => "")
                ),
                (prefix, body, type) => type switch
                {
                    "UL" => ulong.Parse(body, System.Globalization.NumberStyles.HexNumber),
                    "U" => uint.Parse(body, System.Globalization.NumberStyles.HexNumber),
                    "L" => long.Parse(body, System.Globalization.NumberStyles.HexNumber),
                    _ => (object) int.Parse(body, System.Globalization.NumberStyles.HexNumber)
                }
            );

            var dottedNumbers = Rule(
                Optional(Characters("-"), () => ""),
                List(
                    Match<char>(char.IsDigit),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                Match(".", c => "."),
                List(
                    Match<char>(char.IsDigit),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                (neg, whole, dot, fract) => neg + whole + dot + fract
            );

            var formattedDottedNumbers = Rule(
                dottedNumbers,
                First(
                    Characters("F"),
                    Characters("M"),
                    Produce<char, string>(() => "")
                ),
                (value, type) => type switch
                {
                    "F" => float.Parse(value),
                    "M" => decimal.Parse(value),
                    _ => (object) double.Parse(value)
                }
            );

            var integralNumbers = Rule(
                Optional(Characters("-"), () => ""),
                List(
                    Match<char>(char.IsDigit),
                    c => new string(c.ToArray()),
                    atLeastOne: true
                ),
                First(
                    Match("UL", c => "UL"),
                    Match("U", c => "U"),
                    Match("L", c => "L"),
                    Produce<char, string>(() => "")
                ),
                (neg, whole, type) => type switch
                {
                    "UL" => ulong.Parse(whole),
                    "U" => uint.Parse(whole),
                    "L" => long.Parse(whole),
                    _ => (object) int.Parse(whole)
                }
            );

            var hexCharLiterals = Rule(
                Characters("\\x"),
                List(
                    hexCharacters,
                    t => (char) int.Parse(new string(t.ToArray()))
                ),
                (escape, c) => c.ToString()
            );

            var utf16CharLiterals = Rule(
                Characters("\\u"),
                List(
                    // TODO: exactly 4 of these
                    hexCharacters,
                    t => char.ConvertFromUtf32(int.Parse(new string(t.ToArray())))
                ),
                (escape, c) => c
            );

            var utf32CharLiterals = Rule(
                Characters("\\U"),
                List(
                    // TODO: exactly 8 of these
                    hexCharacters,
                    t => char.ConvertFromUtf32(int.Parse(new string(t.ToArray())))
                ),
                (escape, c) => c
            );

            var escapeChars = Rule(
                Characters("\\"),
                Match<char, string>(c => _escapeChars.Contains(c), c => c.ToString()),
                (slash, c) => c
            );

            var stringCharacterLiterals = First(
                hexCharLiterals,
                utf16CharLiterals,
                utf32CharLiterals,
                escapeChars,
                Match<char, string>(c => c != '\0' && c != '"', c => c.ToString())
            );

            var stringBody = List(
                stringCharacterLiterals,
                s => string.Join("", s)
            );

            // strings = """ (<hexSequence> | <utf16Char> | <utf32Char> | <escapedChar> | .)* """
            var strings = Rule(
                Characters("\""),
                stringBody,
                RequireCharacters("\""),
                (start, body, end) => body
            );

            var characterLiterals = First(
                hexCharLiterals,
                utf16CharLiterals,
                escapeChars,
                Transform(Match<char>(c => c != '\0' && c != '\''), c => c.ToString()),
                Error<char, string>(t => $"Expected char value but found {t.Peek()}")
            );

            // chars = "'" ("\" "x" <hexdigit>+ | "\" . | .) "'"
            var chars = Rule(
                Characters("'"),
                characterLiterals,
                RequireCharacters("'"),
                (start, content, end) => (object) content[0]
            );

            return First(
                hexNumbers,
                integralNumbers,
                formattedDottedNumbers,
                strings,
                chars,
                Match("true", c => (object) true),
                Match("false", c => (object) false),
                Match("null", c => (object) null)
            );
        }

        private IParser<char, IPath> InitializePropertiesParser()
        {
            var namedProperty = Rule(
                Match(".", c => c),
                _identifiers,
                (dot, name) =>  new NamedPropertyPath(name)
            );

            var allPropertiesNested = Match("*", t => new AllPropertiesNestedPath());

            var allProperties = Match(".", t => new AllPropertiesPath());

            // property = "*" | "." <identifier> | "."
            return First<char, IPath>(
                allPropertiesNested,
                namedProperty,
                allProperties
            );
        }

        private IParser<char, IPath> InitializePredicatesParser()
        {
            var integer = List(
                Match<char>(char.IsDigit),
                c => int.Parse(new string(c.ToArray())),
                atLeastOne: true
            );

            var predicateExpression = First(
                Rule(
                    LeadingWhitespace(
                        First(
                            Characters("=="),
                            Characters("="),
                            Characters("!=")
                        )
                    ),
                    LeadingWhitespace(_primitives),

                    (op, value) => op switch
                    {
                        "==" => new EqualsPathPredicate(value),
                        "=" => new EqualsPathPredicate(value),
                        "!=" => new NotEqualsPathPredicate(value),
                        _ => (IPathPredicate) new AllPathPredicate()
                    }
                ),
                Produce<char, IPathPredicate>(() => new AllPathPredicate())
            );

            var arityExpression = First(
                Characters("*", c => new AllIfAnyArityComparer()),
                Characters("+", c => new AllAtLeastOneArityComparer()),
                Rule(
                    Characters("{"),
                    integer,
                    Characters("}"),
                    (open, value, close) => new ExactlyArityComparer(value)
                ),
                Rule(
                    Characters("{"),
                    Optional(integer, () => 0),
                    Characters(","),
                    Optional(integer, () => int.MaxValue),
                    Characters("}"),
                    (open, low, comma, high, close) => new RangeArityComparer(low, high)
                ),
                Error<char, IArityComparer>(s => $"Expected repetition specified but found {s.Peek()}")
            );

            var predicateComparePaths = Rule(
                LeadingWhitespace(Characters("{")),
                Require(_singlePath, t => $"Expected path but found {t.Peek()}"),
                predicateExpression,
                LeadingWhitespace(RequireCharacters("}")),
                arityExpression,

                (open, path, predicate, close, arity) => new PredicatePath(path, predicate, arity)
            );
            return predicateComparePaths;
        }

        private IParser<char, IPath> InitializeTypeConstraintsParser()
        {
            // TODO: Generic and array types

            IParser<char, ITypeDescriptor> _typesInternal = null;
            IParser<char, ITypeDescriptor> types = Deferred(() => _typesInternal);

            // <identifier>
            var simpleTypes = Transform(
                _identifiers, 
                name => 
                    (ITypeDescriptor) new SimpleTypeDescriptor(name)
            );

            // <identifier> ("<" <types>+ ">")?
            var genericTypes = Apply(
                simpleTypes,
                // TODO: Would like to be able to specify Type<> unqualified generic type definition
                left => Rule(
                    left,
                    Characters("<"),
                    SeparatedList(
                        types,
                        LeadingWhitespace(Characters(",")),
                        t => t
                    ),
                    RequireCharacters(">"),

                    (type, open, gentypes, close) => new GenericTypeDescriptor(type, gentypes)
                )
            );

            // <genericType> ("." <genericType>)*
            var qualifiedTypes = Transform(
                SeparatedList(
                    genericTypes,
                    Characters("."),
                    parts => parts,
                    atLeastOne: true
                ),
                t => t.Count == 1 ? t.First() : new QualifiedTypeDescriptor(t)
            );

            // <qualifiedTypes> ("[" ","* "]")?
            var arrayTypes = Apply(
                qualifiedTypes,
                left => Rule(
                    left,
                    Characters("["),
                    List(
                        Characters(","),
                        commas => commas.Count + 1
                    ),
                    RequireCharacters("]"),

                    (elementType, open, dimensions, close) => new ArrayTypeDescriptor(elementType, dimensions)
                )
            );

            _typesInternal = Transform(
                arrayTypes,
                t => new CachingTypeDescriptor(t)
            );

            var requiredType = LeadingWhitespace(
                Require(
                    _typesInternal, 
                    t => $"Unexpected token in type constraint {t.Peek()}"
                )
            );

            // "<" <requiredType> ">"
            var typeConstraintPaths = Rule(
                Characters("<"),
                requiredType,
                //genericTypes,
                LeadingWhitespace(RequireCharacters(">")),
                (open, name, close) => (IPath) new TypedPath(name)
            );
            return typeConstraintPaths;
        }

        private IParser<char, IPath> InitializeIndexersParser()
        {
            var commaSeparatedIndices = SeparatedList(
                LeadingWhitespace(_primitives),
                LeadingWhitespace(Characters(",")),
                indices => indices.Count == 0 ? (IPath) new AllIndexerItemsPath() : new IndexerItemsPath(indices)
            );

            var barSeparatedIndiceGroups = SeparatedList(
                commaSeparatedIndices,
                LeadingWhitespace(Characters("|")),
                indiceGroups => indiceGroups.Count == 0 ? indiceGroups.First() : new CombinePath(indiceGroups)
            );

            // indexer = "[" "]" | "[" (<integer> | <string>)+ "]"
            var indexerPaths = Rule(
                Characters("["),
                LeadingWhitespace(barSeparatedIndiceGroups),
                LeadingWhitespace(RequireCharacters("]")),
                (open, indices, close) => indices
            );
            return indexerPaths;
        }

        private IParser<char, TOutput> LeadingWhitespace<TOutput>(IParser<char, TOutput> parser)
            => Rule(
                _whitespace,
                parser,
                (ws, item) => item
            );
    }
}