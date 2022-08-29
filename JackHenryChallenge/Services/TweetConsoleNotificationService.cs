using JackHenryChallenge.Models;
using JackHenryChallenge.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace JackHenryChallenge.Services
{
    /// <summary>
    ///  A NotificationService that notifies to the console
    /// </summary>
    /// <seealso cref="Interfaces.INotificationService" />
    public class TweetConsoleNotificationService : INotificationService
    {
        private readonly ILogger<TweetConsoleNotificationService> _logger;
        public TweetConsoleNotificationService(ILogger<TweetConsoleNotificationService> logger)
        {
            _logger = logger;
        }
        public void Notify(TweetStatistics tweetStatistics)
        {
            var hashTags = string.Join(',', tweetStatistics.Top10HashTags.Where(hashTag => !string.IsNullOrWhiteSpace(hashTag.Tag)).Select(x=> x.Tag));
            if (!tweetStatistics.Top10HashTags.Any())
            {
                hashTags = "No hashtags yet.";
            }
            var output = $"Tweet Count:{tweetStatistics.TweetCount}, Top 10 hashtags:{hashTags}";
            _logger.LogDebug($"TweetConsoleNotificationServiceTweet: {output}");
            Console.WriteLine(output);
        }
    }
}
