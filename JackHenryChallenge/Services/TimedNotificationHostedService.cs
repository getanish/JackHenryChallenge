using JackHenryChallenge.Models;
using JackHenryChallenge.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JackHenryChallenge.Services
{

    /// <summary>
    /// TimedNotificationHostedService that notifies INotificationService 
    /// at the specified interval
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public abstract class TimedNotificationHostedService<T> : IHostedService, IDisposable where T : class
    {
        private readonly ILogger<TimedNotificationHostedService<T>> _logger;
        private readonly INotificationService<T> _notificationService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly AppSettings _appSettings;
        private Timer? _timer = null;

        public TimedNotificationHostedService(
            ILogger<TimedNotificationHostedService<T>> logger,
            INotificationService<T> notificationService,
            IHostApplicationLifetime hostApplicationLifetime,
            IOptions<AppSettings> options)
        {
            _logger = logger;
            _notificationService = notificationService;
            _hostApplicationLifetime = hostApplicationLifetime;
            _appSettings = options.Value;
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
                TimeSpan.FromSeconds(_appSettings.NotificationDurationInSeconds));

            return Task.CompletedTask;
        }
        public abstract T GetNotificationInput();


        private void DoWork(object? state)
        {
            try
            {
                _notificationService.Notify(GetNotificationInput());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TimedNotificationHostedService: failure.");
                _hostApplicationLifetime.StopApplication();
            }
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

    public class TweetStatisticsTimedNotificationHostedService : TimedNotificationHostedService<TweetStatistics>
    {
        private readonly ITweetStatisticsService _tweetStatisticsService;
        public TweetStatisticsTimedNotificationHostedService(
            ILogger<TimedNotificationHostedService<TweetStatistics>> logger, 
            ITweetStatisticsService tweetStatisticsService, 
            INotificationService<TweetStatistics> notificationService,
            IHostApplicationLifetime hostApplicationLifetime,
            IOptions<AppSettings> options) : base(logger, notificationService, hostApplicationLifetime, options)
        {
            _tweetStatisticsService = tweetStatisticsService;
        }

        public override TweetStatistics GetNotificationInput()
        {
            return _tweetStatisticsService.GetTweetStatistics();
        }
    }
}
