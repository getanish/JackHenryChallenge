using JackHenryChallenge.Data.Interfaces;
using JackHenryChallenge.Entities.Twitter;
using JackHenryChallenge.ServiceAgents.Interfaces;
using JackHenryChallenge.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace JackHenryChallenge.Tests.Services
{
    public class ProcessTwitterStreamHostedServiceTests
    {
        readonly TweetData Tweet1 = new TweetData()
        {
            Data = new Tweet
            {
                Id = "1",
                Text = "Text1",
                Entities = new TweetEntities
                {
                    Hashtags = new[]{
                                new HashTag{ Tag="Tag1" },
                                new HashTag{ Tag="Tag2" }
                            }
                }
            }
        };
        readonly TweetData Tweet2 = new TweetData
        {
            Data = new Tweet
            {
                Id = "2",
                Text = "Text2",
                Entities = new TweetEntities
                {
                    Hashtags = new[]{
                                new HashTag{ Tag="Tag2" },
                                new HashTag{ Tag="Tag3" }
                            }
                }
            }
        };

        [Fact]
        public async Task ProcessTwitterStreamHostedService_AddsTweetsToRepositroy()
        {
            var mockLogger = new Mock<ILogger<ProcessTwitterStreamHostedService>>();
            var mockTwitterSa = new Mock<ITwitterSa>();
            var mockRepository = new Mock<ITweetRepository>();
            var mockHost = new Mock<IHostApplicationLifetime>();

            var service = new ProcessTwitterStreamHostedService(mockLogger.Object, mockTwitterSa.Object, mockRepository.Object, mockHost.Object);


            var hashTags = new[] { 
                new TwitterResponse<Tweet>(){ Data = Tweet1.Data },
                new TwitterResponse<Tweet>(){ Data = Tweet2.Data }
            };
            mockTwitterSa.Setup(x => x.GetSampleStream(It.IsAny<GetSampleStreamRequest>(), CancellationToken.None))
                .Returns(hashTags.ToAsyncEnumerable());

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            mockRepository.Verify(x => x.AddTweet(It.IsAny<Tweet>()), Times.Exactly(2));
            mockRepository.Verify(x => x.AddTweet(It.Is<Tweet>(t => t.Id == "1")), Times.Once);
            mockRepository.Verify(x => x.AddTweet(It.Is<Tweet>(t => t.Id == "2")), Times.Once);


        }

        [Fact]
        public async Task ProcessTwitterStreamHostedService_DoesNotAddEmptyTweetStream()
        {
            var mockLogger = new Mock<ILogger<ProcessTwitterStreamHostedService>>();
            var mockTwitterSa = new Mock<ITwitterSa>();
            var mockRepository = new Mock<ITweetRepository>();
            var mockHost = new Mock<IHostApplicationLifetime>();
            var service = new ProcessTwitterStreamHostedService(mockLogger.Object, mockTwitterSa.Object, mockRepository.Object,mockHost.Object);

            var hashTags = new TwitterResponse<Tweet>[] { };
            //var hashTags = new TweetData[] { };
            mockTwitterSa.Setup(x => x.GetSampleStream(It.IsAny<GetSampleStreamRequest>()  ,CancellationToken.None))
                .Returns(hashTags.ToAsyncEnumerable());

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            mockRepository.Verify(x => x.AddTweet(It.IsAny<Tweet>()), Times.Never);


        }
        [Fact]
        public async Task ProcessTwitterStreamHostedService_StopsApplicationAtEndOfStream()
        {
            var mockLogger = new Mock<ILogger<ProcessTwitterStreamHostedService>>();
            var mockTwitterSa = new Mock<ITwitterSa>();
            var mockRepository = new Mock<ITweetRepository>();
            var mockHost = new Mock<IHostApplicationLifetime>();
            var service = new ProcessTwitterStreamHostedService(mockLogger.Object, mockTwitterSa.Object, mockRepository.Object, mockHost.Object);

            var hashTags = new TwitterResponse<Tweet>[] { };
            //var hashTags = new TweetData[] { };
            mockTwitterSa.Setup(x => x.GetSampleStream(It.IsAny<GetSampleStreamRequest>(), CancellationToken.None))
                .Returns(hashTags.ToAsyncEnumerable());

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            mockHost.Verify(x => x.StopApplication(), Times.Once);


        }
    }
}
