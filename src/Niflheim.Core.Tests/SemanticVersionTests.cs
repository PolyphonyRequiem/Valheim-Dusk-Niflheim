using Niflheim.Installer;
using Xunit;

namespace Niflheim.Core.Tests
{
    public class SemanticVersionTests
    {

        [Theory]
        [InlineData("1.0", "1.0.0", 0)]
        [InlineData("1.0.0", "1.0", 0)]
        [InlineData("1.0.0", "1.0.1", -1)]
        [InlineData("1.0", "1.0.1", -1)]
        [InlineData("1.0.1", "1.0", 1)]
        [InlineData("1.0.1", "1.0.0", 1)]
        [InlineData("1.0.0", "1.1", -1)]
        [InlineData("1.0", "1.1.0", -1)]
        [InlineData("1.1.0", "1.0", 1)]
        [InlineData("1.1", "1.0.0", 1)]
        [InlineData("10.20", "30.40", -1)]
        [InlineData("2.1", "1.2", 1)]
        public void TestSemanticVersionsComparisons(string x, string y, int expected)
        {
            var vx = SemanticVersion.Parse(x);
            var vy = SemanticVersion.Parse(y);
            Assert.True(vx.CompareTo(vy) == expected);
            if (expected == 0)
            {
                Assert.True(vx >= vy);
                Assert.True(vx <= vy);
            }

            if (expected == -1)
            {
                Assert.True(vx <= vy);
                Assert.True(vx < vy);
            }

            if (expected == 1)
            {
                Assert.True(vx >= vy);
                Assert.True(vx > vy);
            }
        }
    }
}

