using JackHenryChallenge.Models;

namespace JackHenryChallenge.Services.Interfaces
{
    /// <summary>
    /// Service for Tweet Statistics
    /// </summary>
    public interface ITweetStatisticsService
    {
        /// <summary>
        /// Generates the tweet statistics.
        /// </summary>
        /// <returns></returns>
        TweetStatistics GenerateTweetStatistics();
    }
}
