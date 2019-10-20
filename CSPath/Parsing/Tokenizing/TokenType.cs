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
        OpenBracket,
        CloseBracket,
        OpenParen,
        CloseParen,
        OpenAngle,
        CloseAngle,
        Comma,

        Integer,
        UInteger,
        Decimal,
        Float,
        Double,
        Long,
        ULong,

        Character,
        String
    }
}