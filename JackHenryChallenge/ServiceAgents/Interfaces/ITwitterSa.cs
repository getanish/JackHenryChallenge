using JackHenryChallenge.Models;
using JackHenryChallenge.Models.Twitter;

namespace JackHenryChallenge.ServiceAgents.Interfaces
{
    /// <summary>
    /// Twitter Service agent that talks with twitter service
    /// </summary>
    public interface ITwitterSa
    {
        //IAsyncEnumerable<TweetData> GetSampleStream(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the twitter sample stream.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        IAsyncEnumerable<TwitterResponse<Tweet>> GetSampleStream(GetSampleStreamRequest request, CancellationToken cancellationToken = default);

    }
}