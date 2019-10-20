using CSPath.Parsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests.Tokenizing
{
    public class TokenizerTests
    {
        [Test]
        public void GetNext_Symbols()
        {
            var target = new Tokenizer(".///*|()[]");
            target.GetNext().Type.Should().Be(TokenType.Dot);
            target.GetNext().Type.Should().Be(TokenType.DoubleForwardSlash);
            target.GetNext().Type.Should().Be(TokenType.ForwardSlash);
            target.GetNext().Type.Should().Be(TokenType.Star);
            target.GetNext().Type.Should().Be(TokenType.Bar);
            target.GetNext().Type.Should().Be(TokenType.OpenParen);
            target.GetNext().Type.Should().Be(TokenType.CloseParen);
            target.GetNext().Type.Should().Be(TokenType.OpenBracket);
            target.GetNext().Type.Should().Be(TokenType.CloseBracket);
        }

        [Test]
        public void GetNext_Char()
        {
            PathToken result;
            result = new Tokenizer("'x'").GetNext();
            result.Type.Should().Be(TokenType.Character);
            result.Value.Should().Be("x");

            result = new Tokenizer(@"'\\'").GetNext();
            result.Type.Should().Be(TokenType.Character);
            result.Value.Should().Be(@"\");

            result = new Tokenizer("'\x41'").GetNext();
            result.Type.Should().Be(TokenType.Character);
            result.Value.Should().Be("A");
        }

        [Test]
        public void GetNext_String()
        {
            PathToken result;
            result = new Tokenizer("\"test\"").GetNext();
            result.Type.Should().Be(TokenType.String);
            result.Value.Should().Be("test");

            result = new Tokenizer("\"\\\\\"").GetNext();
            result.Type.Should().Be(TokenType.String);
            result.Value.Should().Be(@"\");

            result = new Tokenizer("\"\x41\"").GetNext();
            result.Type.Should().Be(TokenType.String);
            result.Value.Should().Be("A");
        }

        [Test]
        public void GetNext_Numbers()
        {
            PathToken result;
            result = new Tokenizer("100").GetNext();
            result.Type.Should().Be(TokenType.Integer);
            result.Value.Should().Be("100");

            result = new Tokenizer(@"-100").GetNext();
            result.Type.Should().Be(TokenType.Integer);
            result.Value.Should().Be(@"-100");

            result = new Tokenizer("3.14").GetNext();
            result.Type.Should().Be(TokenType.Double);
            result.Value.Should().Be("3.14");

            result = new Tokenizer("3.14F").GetNext();
            result.Type.Should().Be(TokenType.Float);
            result.Value.Should().Be("3.14");

            result = new Tokenizer("3.14M").GetNext();
            result.Type.Should().Be(TokenType.Decimal);
            result.Value.Should().Be("3.14");

            result = new Tokenizer("100U").GetNext();
            result.Type.Should().Be(TokenType.UInteger);
            result.Value.Should().Be("100");

            result = new Tokenizer("100L").GetNext();
            result.Type.Should().Be(TokenType.Long);
            result.Value.Should().Be("100");

            result = new Tokenizer("100UL").GetNext();
            result.Type.Should().Be(TokenType.ULong);
            result.Value.Should().Be("100");
        }
    }
}