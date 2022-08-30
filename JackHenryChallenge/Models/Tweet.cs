namespace JackHenryChallenge.Models
{
    public class Tweet
    {
        public string Id { get; set; } = null!;
        public string Text { get; set; } = null!;
        public TweetEntities? Entities { get; set; }
    }
}