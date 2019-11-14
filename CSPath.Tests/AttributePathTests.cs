using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class AttributePathTests
    {
        private class TestClass1
        {
            [System.ComponentModel.Description]
            public string Ok => "OK";

            public string Fail => "FAIL";
        }

        [Test]
        public void Path_Object_HasAttributes()
        {
            var target = new TestClass1();
            var result = target.Path(".{@<DescriptionAttribute>}+").ToList();
            result.Count.Should().Be(1);
            result[0].Should().Be("OK");
        }
    }
}
