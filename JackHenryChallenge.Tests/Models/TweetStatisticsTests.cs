using JackHenryChallenge.Models;

namespace JackHenryChallenge.Tests.Models
{
    public class TweetStatisticsTests
    {
        [Fact]
        public void Test1()
        {
            var a = new TweetStatistics();
            Assert.Equal(0, a.TweetCount);
            Assert.NotNull(a.Top10HashTags);

        }
    }
}
