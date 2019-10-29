using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class PropertyPathTests
    {
        public class TestClass1
        {
            public int IntValue { get; } = 5;
            public string StringValue { get; } = "test";
            public object ObjectValue { get; } = new object();
        }

        [Test]
        public void Path_Root()
        {
            var target = new TestClass1();
            target.Path("").Single().Should().Be(target);
            target.Path(null).Single().Should().Be(target);
        }

        [Test]
        public void Path_AllProperties()
        {
            var result = new TestClass1().Path(".").ToList();
            result[0].Should().Be(5);
            result[1].Should().Be("test");
            result[2].Should().NotBeNull();
        }

        [Test]
        public void Path_NamedProperty()
        {
            new TestClass1().Path(".IntValue").Single().Should().Be(5);
        }

        public class TestClass2
        {
            public TestClass1 Value { get; } = new TestClass1();
        }

        [Test]
        public void Path_ChildProperty()
        {
            new TestClass2().Path(".Value.StringValue").Single().Should().Be("test");
        }

        [Test]
        public void Path_ChildProperty_Multiline()
        {
            new TestClass2().Path(@"
                .Value.StringValue
            ").Single().Should().Be("test");
        }
    }
}
