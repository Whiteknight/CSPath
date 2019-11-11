using System.Collections.Generic;
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

        public class TestClass2
        {
            public List<string> Strings { get; set; }
            public List<int> Integers { get; set; }
        }

        [Test]
        public void Path_Typed_Generic1()
        {
            var target = new TestClass2
            {
                Strings = new List<string> { "A", "B" },
                Integers = new List<int> { 1, 2 }
            };
            var result = target.Path(".<List<string>>[]").ToList();
            result.Count.Should().Be(2);
            result[0].Should().Be("A");
            result[1].Should().Be("B");
        }

        public class A<T1>
        {
            public class C<T2>
            {
                public override string ToString()
                {
                    return "C" + typeof(T2).Name;
                }
            }
        }

        public class B
        {
        }

        public class D
        {
        }

        public class E<T>
        {
            public class G
            {
                public override string ToString() => "G";
            }
        }

        public class F
        {
        }

        [Test]
        public void Path_Typed_Generic2()
        {
            var target = new object[]
            {
                "FAIL",
                0,
                new E<F>.G(),
                new A<B>.C<D>()
            };
            var result = target.Path("[]<A<B>.C<D>>").ToList();
            result.Count.Should().Be(1);
            result[0].ToString().Should().Be("CD");

            result = target.Path("[]<W<X>.Y<Z>>").ToList();
            result.Count.Should().Be(0);

            result = target.Path("[]<E<F>.G>").ToList();
            result.Count.Should().Be(1);
            result[0].ToString().Should().Be("G");

            result = target.Path("[]<W<X>.Y>").ToList();
            result.Count.Should().Be(0);
        }
    }
}
