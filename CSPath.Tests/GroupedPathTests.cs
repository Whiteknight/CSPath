using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class GroupedPathTests
    {
        private class TypeA
        {
            public TypeB B1 { get; set; }
            public TypeB B2 { get; set; }
            public TypeB B3 { get; set; }
        }

        private class TypeB
        {
            public string Value { get; }

            public TypeB(string value )
            {
                Value = value;
            }
        }

        [Test]
        public void Path_Grouped()
        {
            var target = new TypeA
            {
                B1 = new TypeB("1"),
                B2 = new TypeB("2"),
                B3 = new TypeB("FAIL")
            };
            var result = target.Path("(.B1 | .B2).Value").ToList();
            result.Count.Should().Be(2);
            result[0].ToString().Should().Be("1");
            result[1].ToString().Should().Be("2");
        }
    }
}
