using System.Diagnostics.CodeAnalysis;

namespace JackHenryChallenge.Entities
{
    public class Tweet
    {
        public string Id { get; set; } = null!;
        public string Text { get; set; } = null!;
        public TweetEntities? Entities { get; set; }
    }
}