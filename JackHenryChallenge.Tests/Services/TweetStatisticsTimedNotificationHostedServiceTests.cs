using JackHenryChallenge.Models;
using JackHenryChallenge.Services;
using JackHenryChallenge.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JackHenryChallenge.Tests.Services
{
    public class TweetStatisticsTimedNotificationHostedServiceTests
    {
        [Fact]
        public async Task TimedNotificationHostedService_NotifiesOnInternval()
        {
            var mockLogger = new Mock<ILogger<TimedNotificationHostedService<TweetStatistics>>>();
            var mockStatisticsService = new Mock<ITweetStatisticsService>();
            var mockNotificationService = new Mock<INotificationService<TweetStatistics>>();
            var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
            var mockOptions = Options.Create(new AppSettings()
            {
                NotificationDurationInSeconds=2
            }); ;
            
            mockStatisticsService.Setup(x=> x.GetTweetStatistics()).Returns(new TweetStatistics());
            var service = new TweetStatisticsTimedNotificationHostedService(
                mockLogger.Object, 
                mockStatisticsService.Object,
                mockNotificationService.Object,
                hostApplicationLifetime.Object,
                mockOptions);
            await service.StartAsync(CancellationToken.None);
            await Task.Delay(TimeSpan.FromSeconds(6));
            await service.StopAsync(CancellationToken.None);
            mockNotificationService.Verify(x=> x.Notify(It.IsAny<TweetStatistics>()), Times.AtLeast(3));
            hostApplicationLifetime.Verify(x => x.StopApplication(), Times.Never());

        }

        [Fact]
        public async Task TimedNotificationHostedService_StopsApplicationOnError()
        {
            var mockLogger = new Mock<ILogger<TimedNotificationHostedService<TweetStatistics>>>();
            var mockStatisticsService = new Mock<ITweetStatisticsService>();
            var mockNotificationService = new Mock<INotificationService<TweetStatistics>>();
            var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
            var mockOptions = Options.Create(new AppSettings()
            {
                NotificationDurationInSeconds = 2
            }); ;

            mockStatisticsService.Setup(x => x.GetTweetStatistics()).Throws(new Exception("Test"));

            var service = new TweetStatisticsTimedNotificationHostedService(
                mockLogger.Object,
                mockStatisticsService.Object,
                mockNotificationService.Object,
                hostApplicationLifetime.Object,
                mockOptions);

            
            await service.StartAsync(CancellationToken.None);
            await Task.Delay(TimeSpan.FromSeconds(3));
            await service.StopAsync(CancellationToken.None);
            mockNotificationService.Verify(x => x.Notify(It.IsAny<TweetStatistics>()), Times.Never());
            hostApplicationLifetime.Verify(x=>x.StopApplication(), Times.AtLeastOnce());

        }
    }
}
