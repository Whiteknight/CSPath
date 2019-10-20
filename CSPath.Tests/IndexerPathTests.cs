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
        public void Path_Array_IndexItems()
        {
            var result = new[] { 1, 2, 3, 4 }.Path("[0, 2]").ToList();
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