using JackHenryChallenge.Entities;

namespace JackHenryChallenge.Models
{
    public class TweetStatistics
    {
        public int TweetCount { get; set; }
        public IEnumerable<HashTag> Top10HashTags { get; set; } = Enumerable.Empty<HashTag>();

    }
}
