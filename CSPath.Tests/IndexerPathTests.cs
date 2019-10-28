using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class IndexerPathTests
    {
        [Test]
        public void Path_Array_AllIndices()
        {
            var result = new[] { 1, 2, 3, 4 }.Path("[]").ToList();
            result.Should().BeEquivalentTo(1, 2, 3, 4);
        }

        [Test]
        public void Path_Array_IndexItem()
        {
            var result = new[] { 1, 2, 3, 4 }.Path("[0]").ToList();
            result.Should().BeEquivalentTo(1);
        }

        [Test]
        public void Path_Array_IndexItem2D()
        {
            var data = new[,] {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            data.Path("[0,2]").Single().Should().Be(3);
            data.Path("[1,1]").Single().Should().Be(5);
            data.Path("[2,0]").Single().Should().Be(7);
        }

        [Test]
        public void Path_Array_IndexItem3D()
        {
            var data = new int[,,] { 
                { 
                    { 1, 2, 3 }, 
                    { 4, 5, 6 } 
                },
                { 
                    { 7, 8, 9 }, 
                    { 10, 11, 12 } 
                } 
            };
            data.Path("[0,1,2]").Single().Should().Be(6);
            data.Path("[1,0,1]").Single().Should().Be(8);
            data.Path("[1,1,1]").Single().Should().Be(11);
        }

        public class IntIndexableItem
        {
            public int this[int d] => d;
            public int this[int c, int d] => (c * 10) + d;
            public int this[int b, int c, int d] => (b * 100) + (c * 10) + d;
            public int this[int a, int b, int c, int d] => (a * 1000) + (b * 100) + (c * 10) + d;
        }

        [Test]
        public void Path_ObjectIndexers_ItemItems()
        {
            var data = new IntIndexableItem();
            data.Path("[5]").Single().Should().Be(5);
            data.Path("[2,5]").Single().Should().Be(25);
            data.Path("[1,2,5]").Single().Should().Be(125);
            data.Path("[8,1,2,5]").Single().Should().Be(8125);
        }

        public class MultiIndexableItem
        {
            public int this[int a] => a * 10;
            public int this[string s] => s.Length;
        }

        [Test]
        public void Path_ObjectIndexers_MultipleTypes()
        {
            var data = new MultiIndexableItem();
            data.Path("[5 | \"test\"]").ToList().Should().BeEquivalentTo(50, 4);
        }

        [Test]
        public void Path_Array_IndexItems()
        {
            var result = new[] { 1, 2, 3, 4 }.Path("[0 | 2]").ToList();
            result.Should().BeEquivalentTo(1, 3);

            // Above is just a shorthand for this:
            result = new[] { 1, 2, 3, 4 }.Path("[0] | [2]").ToList();
            result.Should().BeEquivalentTo(1, 3);
        }

        [Test]
        public void Path_Dictionary_AllIndexer()
        {
            var dict = new Dictionary<string, object>
            {
                { "A", 5 },
                { "B", "test" },
                { "C", 3.14 }
            };
            var result = dict.Path("[]").ToArray();
            result.Length.Should().Be(3);

            result = dict.Path("[].Key").ToArray();
            result.Should().BeEquivalentTo(new [] { "A", "B", "C" });

            result = dict.Path("[].Value").ToArray();
            result.Should().BeEquivalentTo(5, "test", 3.14);
        }

        [Test]
        public void Path_Dictionary_IndexByStringKey()
        {
            var dict = new Dictionary<string, object>
            {
                { "A", 5 },
                { "B", "test" },
                { "C", 3.14 }
            };
            dict.Path("[\"A\"]").Single().Should().Be(5);
        }
    }
}