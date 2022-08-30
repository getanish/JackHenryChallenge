using JackHenryChallenge.Models;

namespace JackHenryChallenge.Services.Interfaces
{
    /// <summary>
    /// ITweetStatisticsService
    /// </summary>
    public interface ITweetStatisticsService
    {
        /// <summary>
        /// Gets the tweet statistics.
        /// </summary>
        /// <returns></returns>
        TweetStatistics GetTweetStatistics();
    }
}
