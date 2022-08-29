using JackHenryChallenge.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JackHenryChallenge.Services
{
    /// <summary>
    /// TimedNotificationHostedService that notifies INotificationService 
    /// at the specified interval
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public class TimedNotificationHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedNotificationHostedService> _logger;
        private readonly ITweetStatisticsService _tweetStatisticsService;
        private readonly INotificationService _notificationService;
        private Timer? _timer = null;

        public TimedNotificationHostedService(
            ILogger<TimedNotificationHostedService> logger,
            ITweetStatisticsService tweetStatisticsService,
            INotificationService notificationService)
        {
            _logger = logger;
            _tweetStatisticsService = tweetStatisticsService;
            _notificationService = notificationService;
        }
        /// <summary>
        /// Starts the asynchronous process to notify.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting notification service.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(10)); // This should be from the config.

            return Task.CompletedTask;
        }
        private void DoWork(object? state)
        {
            _notificationService.Notify(_tweetStatisticsService.GenerateTweetStatistics());

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping notification service.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
