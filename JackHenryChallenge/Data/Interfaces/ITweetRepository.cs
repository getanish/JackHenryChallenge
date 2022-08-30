using JackHenryChallenge.Models;

namespace JackHenryChallenge.Data.Interfaces
{
    /// <summary>
    /// ITweetRepository
    /// </summary>
    public interface ITweetRepository
    {
        /// <summary>
        /// Gets the tweet count.
        /// </summary>
        /// <value>
        /// The tweet count.
        /// </value>
        int TweetCount { get; }
        /// <summary>
        /// Adds the tweet.
        /// </summary>
        /// <param name="tweet">The tweet.</param>
        void AddTweet(Tweet tweet);
        /// <summary>
        /// Gets the top hash tags.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IEnumerable<HashTag> GetTopHashTags(int count);
    }
}
