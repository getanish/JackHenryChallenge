using JackHenryChallenge.Data.Interfaces;
using JackHenryChallenge.Models;
using JackHenryChallenge.Services.Interfaces;

namespace JackHenryChallenge.Services
{
    /// <summary>
    /// A TweetStatisticsService that creates tweet statistics
    /// </summary>
    /// <seealso cref="Interfaces.INotificationInput" />
    public class TweetStatisticsService : ITweetStatisticsService
    {
        private readonly ITweetRepository _tweetRepository;

        public TweetStatisticsService(ITweetRepository tweetRepository)
        {
            _tweetRepository = tweetRepository;

        }
        /// <summary>
        /// Generates the tweet statistics.
        /// </summary>
        /// <returns></returns>
        public TweetStatistics GetTweetStatistics()
        {
            return new TweetStatistics
            {
                TweetCount = _tweetRepository.TweetCount,
                Top10HashTags = _tweetRepository.GetTopHashTags(10)
            };

        }
    }
}
