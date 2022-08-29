using JackHenryChallenge.Models;
using JackHenryChallenge.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace JackHenryChallenge.Tests.Services
{
    public class TweetConsoleNotificationServiceTests
    {
        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenNoHashTagsArePresent()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var mockLogger = new Mock<ILogger<TweetConsoleNotificationService>>();
                var service = new TweetConsoleNotificationService(mockLogger.Object);
                service.Notify(new TweetStatistics{ TweetCount=0 });

                string expected = string.Format("Tweet Count:0, Top 10 hashtags:No hashtags yet.{0}", Environment.NewLine);
                Assert.Equal(expected, sw.ToString());
            }
        }

        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenHashTagsArePresent()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var mockLogger = new Mock<ILogger<TweetConsoleNotificationService>>();
                var service = new TweetConsoleNotificationService(mockLogger.Object);
                service.Notify(new TweetStatistics { TweetCount = 0 ,  Top10HashTags = new List<HashTag>()
                {
                    new HashTag() { Tag = "Tag1"},
                    new HashTag() { Tag = "Tag2"}
                }
                });

                string expected = string.Format("Tweet Count:0, Top 10 hashtags:Tag1,Tag2{0}", Environment.NewLine);
                Assert.Equal(expected, sw.ToString());
            }
        }

        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenHashTagsArePresentWithEmptyTag()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var mockLogger = new Mock<ILogger<TweetConsoleNotificationService>>();
                var service = new TweetConsoleNotificationService(mockLogger.Object);
                service.Notify(new TweetStatistics
                {
                    TweetCount = 0,
                    Top10HashTags = new List<HashTag>()
                {
                    new HashTag() { Tag = "Tag1"},
                    new HashTag() { Tag = string.Empty}
                }
                });

                string expected = string.Format("Tweet Count:0, Top 10 hashtags:Tag1{0}", Environment.NewLine);
                Assert.Equal(expected, sw.ToString());
            }
        }


        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenNoHashTagsArePresentAndNonZeroCount()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var mockLogger = new Mock<ILogger<TweetConsoleNotificationService>>();
                var service = new TweetConsoleNotificationService(mockLogger.Object);
                service.Notify(new TweetStatistics
                {
                    TweetCount = 0100
                });

                string expected = string.Format("Tweet Count:100, Top 10 hashtags:No hashtags yet.{0}", Environment.NewLine);
                Assert.Equal(expected, sw.ToString());
            }
        }

        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenHashTagsArePresentWithNonZeroCount()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var mockLogger = new Mock<ILogger<TweetConsoleNotificationService>>();
                var service = new TweetConsoleNotificationService(mockLogger.Object);
                service.Notify(new TweetStatistics
                {
                    TweetCount = 227,
                    Top10HashTags = new List<HashTag>()
                    {
                        new HashTag() { Tag = "Tag1"},
                        new HashTag() { Tag = "Tag2"}
                    }
                });

                string expected = string.Format("Tweet Count:227, Top 10 hashtags:Tag1,Tag2{0}", Environment.NewLine);
                Assert.Equal(expected, sw.ToString());
            }
        }

        [Fact]
        public void TweetConsoleNotificationService_NotifiesWhenHashTagsArePresentWithEmptyTagWithNonZeroCount()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var mockLogger = new Mock<ILogger<TweetConsoleNotificationService>>();
                var service = new TweetConsoleNotificationService(mockLogger.Object);
                service.Notify(new TweetStatistics
                {
                    TweetCount = 1898291,
                    Top10HashTags = new List<HashTag>()
                    {
                        new HashTag() { Tag = "Tag1"},
                        new HashTag() { Tag = string.Empty}
                    }
                });

                string expected = string.Format("Tweet Count:1898291, Top 10 hashtags:Tag1{0}", Environment.NewLine);
                Assert.Equal(expected, sw.ToString());
            }
        }
    }
}
