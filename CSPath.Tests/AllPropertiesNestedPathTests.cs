using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class AllPropertiesNestedPathTests
    {
        private class Circular
        {
            public Circular Reference { get; set; }
        }

        [Test]
        public void Path_CircularReference()
        {
            var obj = new Circular();
            obj.Reference = obj;
            var result = obj.Path("*").ToList();
            result.Count.Should().Be(1);
            result[0].Should().BeSameAs(obj);
        }

        public class WithChildren
        {
            public object ValueA { get; set; }
            public object ValueB { get; set; }
            public object ValueC { get; set; }
        }

        [Test]
        public void Path_GetAllNestedValues()
        {
            var target = new WithChildren
            {
                ValueA = 1,
                ValueB = new WithChildren
                {
                    ValueA = 2,
                    ValueB = 3,
                    ValueC = 4
                },
                ValueC = new WithChildren
                {
                    ValueA = new WithChildren
                    {
                        ValueA = new WithChildren
                        {
                            ValueA = new WithChildren
                            {
                                ValueA = 5
                            }
                        }
                    }
                }
            };
            var result = target.Path("*").Where(x => x != null).OfType<int>().ToList();
            result.Should().BeEquivalentTo(1, 2, 3, 4, 5);
        }

        [Test]
        public void Path_GetAllNestedValuesThenProperty()
        {
            var target = new WithChildren
            {
                ValueA = 1,
                ValueB = new WithChildren
                {
                    ValueA = 2,
                    ValueB = 3,
                    ValueC = 4
                },
                ValueC = new WithChildren
                {
                    ValueA = new WithChildren
                    {
                        ValueA = new WithChildren
                        {
                            ValueA = new WithChildren
                            {
                                ValueA = 5
                            }
                        }
                    }
                }
            };
            // "*" doesn't return the root object itself, so we only return children of root
            // and only ValueA (then we filter by int) to get 2, 5
            var result = target.Path("*.ValueA").Where(x => x != null).OfType<int>().ToList();
            result.Should().BeEquivalentTo(2, 5);

        }
    }
}
