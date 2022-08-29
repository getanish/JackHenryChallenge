using JackHenryChallenge.Models;

namespace JackHenryChallenge.Services.Interfaces
{
    /// <summary>
    /// A service that provides notification services
    /// </summary>
    public interface INotificationService
    {
        void Notify(TweetStatistics tweetStatistics);
    }
}
