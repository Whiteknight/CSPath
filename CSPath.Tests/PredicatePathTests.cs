using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class PredicatePathTests
    {
        public class TestClass
        {
            public TestClass(int value)
            {
                ChangingValue = value;
            }

            public int ChangingValue { get; }
            public int FixedValue { get; } = 5;
        }

        [Test]
        public void Path_Array_PredicateSimple()
        {
            var target = new[]
            {
                new TestClass(1),
                new TestClass(2),
                new TestClass(3)
            };
            var result = target.Path("[]{.ChangingValue = 2}").OfType<TestClass>().Single();
            result.ChangingValue.Should().Be(2);
        }

        [Test]
        public void Path_Array_PredicateAllAtLeastOne()
        {
            var target = new[]
            {
                new TestClass(1),
                new TestClass(2),
                new TestClass(3)
            };
            target.Path("[]{.ChangingValue<string> = 2}").ToList().Should().BeEmpty();
            target.Path("[]{.ChangingValue<Int32> = 2}").ToList().Count.Should().Be(1);
        }

        [Test]
        public void Path_Array_PredicateZeroOrAll()
        {
            var target = new[]
            {
                new TestClass(1),
                new TestClass(2),
                new TestClass(3),
                new TestClass(4),
                new TestClass(5)
            };
            // All int properties should == 2, but FixedValue is 5, so returns nothing
            target.Path("[]{.<Int32> = * 2}").ToList().Count.Should().Be(0);

            // All int properties should == 5, which is true only in the last case
            target.Path("[]{.<Int32> = * 5}").ToList().Count.Should().Be(1);

            // All long properties should == 5, but there are no long properties, so all objects match
            target.Path("[]{.<Int64> = * 5}").ToList().Count.Should().Be(5);
        }

        [Test]
        public void Path_Array_PredicateAtLeastOne()
        {
            var target = new[]
            {
                new TestClass(1),
                new TestClass(2),
                new TestClass(3),
                new TestClass(4),
                new TestClass(5)
            };
            // Only one has an int property with value 2
            target.Path("[]{.<Int32> = + 2}").ToList().Count.Should().Be(1);

            // All of them have an int property with value 5
            target.Path("[]{.<Int32> = + 5}").ToList().Count.Should().Be(5);

            // none of them have a long property with value 5
            target.Path("[]{.<Int64> = + 5}").ToList().Count.Should().Be(0);
        }

        [Test]
        public void Path_Array_PredicateExactlyOne()
        {
            var target = new[]
            {
                new TestClass(1),
                new TestClass(2),
                new TestClass(3),
                new TestClass(4),
                new TestClass(5)
            };
            // Only one has exactly one int property with value 2
            target.Path("[]{.<Int32> = | 2}").ToList().Count.Should().Be(1);

            // the first four have exactly one int property with value 5
            target.Path("[]{.<Int32> = | 5}").ToList().Count.Should().Be(4);

            // none of them have exactly one long property with value 5
            target.Path("[]{.<Int64> = | 5}").ToList().Count.Should().Be(0);
        }

        [Test]
        public void Path_ArrayArray_PredicateTrueFalse()
        {
            var target = new[]
            {
                new object[0]
            };
            // Only one has exactly one int property with value 2
            target.Path("[]{.IsReadOnly = true}").ToList().Count.Should().Be(0);
            target.Path("[]{.IsReadOnly = false}").ToList().Count.Should().Be(1);
        }
    }
}
