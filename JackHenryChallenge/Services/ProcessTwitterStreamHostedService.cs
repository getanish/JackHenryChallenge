using JackHenryChallenge.Data.Interfaces;
using JackHenryChallenge.Entities.Twitter;
using JackHenryChallenge.Models.Twitter;
using JackHenryChallenge.ServiceAgents.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace JackHenryChallenge.Services
{
    /// <summary>
    /// A ProcessTwitterStreamHostedService that reads the tweet stream 
    /// and adds to the repository
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class ProcessTwitterStreamHostedService : BackgroundService
    {
        private readonly ILogger<ProcessTwitterStreamHostedService> _logger;
        private readonly ITwitterSa _twitterSa;
        private readonly ITweetRepository _tweetRepository;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public ProcessTwitterStreamHostedService(
            ILogger<ProcessTwitterStreamHostedService> logger,
            ITwitterSa twitterSa,
            ITweetRepository hashTagStore,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _twitterSa = twitterSa;
            _tweetRepository = hashTagStore;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting ProcessTwitterStreamHostedService");
                var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });
                await foreach (var item in _twitterSa.GetSampleStream(request).WithCancellation(cancellationToken))
                {
                    _logger.LogDebug("ProcessTwitterStreamHostedService: TweetData:{@tweetData}", item);
                    _tweetRepository.AddTweet(item.Data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessTwitterStreamHostedService: Error in processing twitterstream");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
            
        }
    }
}
