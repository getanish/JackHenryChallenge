using JackHenryChallenge.Data.Interfaces;
using JackHenryChallenge.Services;
using Moq;

namespace JackHenryChallenge.Tests.Services
{
    public class TweetStatisticsServiceTests
    {
        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenNoHashTagsArePresent()
        {
            var mockRepo = new Mock<ITweetRepository>();
            mockRepo.Setup(x => x.TweetCount).Returns(2);
            mockRepo.Setup(x => x.GetTopHashTags(10)).Returns(new[]
            {
                new HashTag(){ Tag="1"},
                new HashTag(){ Tag="2"},
                new HashTag(){ Tag="3"}
            }.AsEnumerable());
            var service = new TweetStatisticsService(mockRepo.Object);
            var result = service.GetTweetStatistics();
            Assert.NotNull(result);
            Assert.Equal(2, result.TweetCount);
            Assert.Equal(3, result.Top10HashTags.Count());

        }
    }
}
