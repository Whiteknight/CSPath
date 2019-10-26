using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class TypedPathTests
    {
        public class TestClass1
        {
            public int IntValue { get; } = 5;
            public string StringValue { get; } = "test";
            public object ObjectValue { get; } = new object();
        }

        [Test]
        public void Path_Typed_Short()
        {
            var target = new TestClass1();
            var result = target.Path(".<Int32>").ToList();
            result.Should().BeEquivalentTo(5);
        }

        [Test]
        public void Path_Typed_Long()
        {
            var target = new TestClass1();
            var result = target.Path(".<System.String>").ToList();
            result.Should().BeEquivalentTo(new [] {"test"});
        }
    }
}
