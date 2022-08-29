using JackHenryChallenge.Data.Interfaces;
using JackHenryChallenge.Entities;
using System.Collections.Concurrent;

namespace JackHenryChallenge.Data
{
    /// <summary>
    /// Tweet Repository
    /// </summary>
    /// <seealso cref="Interafaces.ITweetRepository" />
    public class TweetRepository : ITweetRepository
    {
        private readonly ConcurrentDictionary<HashTag, int>
            _dataStore = new ConcurrentDictionary<HashTag, int>();

        private int _tweetCount = 0;
        /// <summary>
        /// Adds the tweet.
        /// </summary>
        /// <param name="tweet">The tweet.</param>
        /// <exception cref="System.ArgumentNullException">tweet</exception>
        public void AddTweet(Tweet tweet)
        {
            if (tweet == null) throw new ArgumentNullException(nameof(tweet));

            Interlocked.Increment(ref _tweetCount);
            if (tweet.Entities?.Hashtags != null)
            {
                foreach (var hashTag in tweet.Entities.Hashtags)
                {
                    _ = AddHashTag(hashTag);
                }
            }
        }
        /// <summary>
        /// Gets the tweet count.
        /// </summary>
        /// <value>
        /// The tweet count.
        /// </value>
        public int TweetCount => _tweetCount;
        /// <summary>
        /// Adds the hash tag.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private int AddHashTag(HashTag key) =>
            _dataStore.AddOrUpdate(key, 1, (k, v) => Interlocked.Increment(ref v));
        /// <summary>
        /// Gets the top hash tags.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IEnumerable<HashTag> GetTopHashTags(int count) =>
            _dataStore.OrderByDescending(x => x.Value).Take(count).Select(x => x.Key);
    }
}
