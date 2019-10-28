namespace CSPath.Parsing.Tokenizing
{
    public enum TokenType
    {
        Unknown,

        EndOfInput,
        Whitespace,

        Dot,
        ForwardSlash,
        DoubleForwardSlash,

        Identifier,

        Bar,
        Star,
        Comma,

        OpenBracket,
        CloseBracket,

        OpenParen,
        CloseParen,

        OpenAngle,
        CloseAngle,

        OpenBrace,
        CloseBrace,

        Equals,
        Plus,

        Integer,
        UInteger,
        Decimal,
        Float,
        Double,
        Long,
        ULong,

        Character,
        String,

        True,
        False,

        Null
    }
}