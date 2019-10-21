using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CSPath.Tests
{
    public class CombinePathTests
    {
        [Test]
        public void Path_Combine()
        {
            new int[] { 1, 2, 3, 4 }.Path("[0] | [3]").ToList().Should().BeEquivalentTo(1, 4);
        }
    }
}
