using JackHenryChallenge.Data;
using JackHenryChallenge.Models;

namespace JackHenryChallenge.Tests.Data
{
    public class TweetRepositoryTests
    {
        private static IEnumerable<HashTag> GetTags(int from , int to)
        {
            for(int i= from; i <= to; i++)
            {
                yield return new HashTag { Tag = $"Tag{i}" };
            }
            
        }
        [Fact]
        public void TweetRepository_InitializesWithZeroCount()
        {
            var repo = new TweetRepository();
            Assert.Equal(0,repo.TweetCount);
        }

        [Fact]
        public void TweetRepository_GetsEmptyHashTagsWhenNoTagsArePresent()
        {
            var repo = new TweetRepository();
            repo.AddTweet(new Tweet());
            Assert.Equal(1, repo.TweetCount);
            Assert.Empty(repo.GetTopHashTags(10));
        }

        [Fact]
        public void TweetRepository_ThrowsExceptionForNullTweet()
        {
            var repo = new TweetRepository();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => repo.AddTweet(tweet: null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
        [Fact]
        public void TweetRepository_GetHashTagsByTagCount()
        {
            var repo = new TweetRepository();
            repo.AddTweet(new Tweet
            {
                Id = "1",
                Text = "Text1",
                Entities = new TweetEntities
                {
                    Hashtags = GetTags(25, 25).ToArray()
                }
            });
            repo.AddTweet(new Tweet
            {
                Id = "2",
                Text = "Text2",
                Entities = new TweetEntities
                {
                    Hashtags = GetTags(24, 25).ToArray()
                }
            });
            repo.AddTweet(new Tweet
            {
                Id = "2",
                Text = "Text2",
                Entities = new TweetEntities
                {
                    Hashtags = GetTags(23, 25).ToArray()
                }
            });
            repo.AddTweet(new Tweet
            {
                Id = "2",
                Text = "Text2",
                Entities = new TweetEntities
                {
                    Hashtags = GetTags(22, 25).ToArray()
                }
            });
            repo.AddTweet(new Tweet
            {
                Id = "2",
                Text = "Text2",
                Entities = new TweetEntities
                {
                    Hashtags = GetTags(21, 25).ToArray()
                }
            });
            repo.AddTweet(new Tweet
            {
                Id = "2",
                Text = "Text2",
                Entities = new TweetEntities
                {
                    Hashtags = GetTags(20, 25).ToArray()
                }
            });
            Assert.Equal(6, repo.TweetCount);
            var hashTags = repo.GetTopHashTags(3).ToArray();
            Assert.Equal("Tag25", hashTags.ElementAt(0).Tag);
            Assert.Equal("Tag24", hashTags.ElementAt(1).Tag);
            Assert.Equal("Tag23", hashTags.ElementAt(2).Tag);
        }
    }
}
