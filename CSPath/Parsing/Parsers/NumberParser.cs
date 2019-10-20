using System.Collections.Generic;
using CSPath.Parsing.Tokenizing;

namespace CSPath.Parsing.Parsers
{
    public class NumberParser : IParser<char, PathToken>
    {
        public (bool success, object value) ParseUntyped(ISequence<char> t) => Parse(t);
        public (bool success, PathToken value) Parse(ISequence<char> t)
        {
            var chars = new List<char>();
            char c;
            bool hasDecimal = false;

            c = t.GetNext();
            chars.Add(c);
            
            while (true)
            {
                c = t.GetNext();
                if (!char.IsNumber(c))
                {
                    t.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            if (t.Peek() == '.')
            {
                var dot = t.GetNext();
                if (!char.IsDigit(t.Peek()))
                {
                    // The . is for method invocation, not a decimal, so we put back
                    t.PutBack(dot);
                    return (true,  new PathToken(new string(chars.ToArray()), TokenType.Integer));
                }

                hasDecimal = true;
                chars.Add(dot);
                while (true)
                {
                    c = t.GetNext();
                    if (!char.IsNumber(c))
                    {
                        t.PutBack(c);
                        break;
                    }

                    chars.Add(c);
                }
            }

            c = t.GetNext();
            if (c == 'F')
                return (true, new PathToken(new string(chars.ToArray()), TokenType.Float));
            if (c == 'M')
                return (true, new PathToken(new string(chars.ToArray()), TokenType.Decimal));
            if (c == 'L' && !hasDecimal)
                return (true, new PathToken(new string(chars.ToArray()), TokenType.Long));
            if (c == 'U' && !hasDecimal)
            {
                c = t.Peek();
                if (c == 'L')
                {
                    t.GetNext();
                    return (true, new PathToken(new string(chars.ToArray()), TokenType.ULong));
                }

                return (true, new PathToken(new string(chars.ToArray()), TokenType.UInteger));
            }

            t.PutBack(c);

            if (hasDecimal)
                return (true, new PathToken(new string(chars.ToArray()), TokenType.Double));
            return (true, new PathToken(new string(chars.ToArray()), TokenType.Integer));
        }
    }
}