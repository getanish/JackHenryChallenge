using JackHenryChallenge.Data;
using JackHenryChallenge.Data.Interfaces;
using JackHenryChallenge.Models;
using JackHenryChallenge.ServiceAgents;
using JackHenryChallenge.ServiceAgents.Interfaces;
using JackHenryChallenge.Services;
using JackHenryChallenge.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using System.Net.Http.Headers;

namespace JackHenryChallenge
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            using (var logger = GetLogger(GetConfiguration()))
            {
                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    Console.WriteLine("JackHenryChallenge:Start");
                    using IHost host = CreateHostBuilder(logger).Build();
                    await host.RunAsync();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Application Exception");
                    Console.WriteLine("Application error: Please refer the logs.");
                }
                finally
                {
                    Console.WriteLine("JackHenryChallenge:End");
                }
            }

            static IConfigurationRoot GetConfiguration()
            {
                return new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                                .Build();
            }

            static Logger GetLogger(IConfigurationRoot configuration)
            {
                return new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .CreateLogger();
            }
        }

        internal static IHostBuilder CreateHostBuilder(Logger logger)
        {
            return Host.CreateDefaultBuilder()
                                .ConfigureServices((context, services) =>
                                {
                                    services.AddOptions();
                                    services.Configure<AppSettings>(
                                        context.Configuration.GetSection("AppSettings"));
                                    services.AddHttpClient<ITwitterSa, TwitterSa>((provider, client) =>
                                    {
                                        var configuration = provider.GetService<IOptions<AppSettings>>()
                                            ?? throw new ArgumentNullException(nameof(provider), "Invalid Configuration");

                                        client.BaseAddress = new Uri(configuration.Value.TwitterConfig.BaseUrl);
                                        client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.Value.TwitterConfig.BearerToken);
                                    });
                                    services.AddHostedService<ProcessTwitterStreamHostedService>();
                                    services.AddHostedService<TweetStatisticsTimedNotificationHostedService>();
                                    
                                    services.AddScoped<ITweetStatisticsService, TweetStatisticsService>();
                                    services.AddSingleton<ITweetRepository, TweetRepository>();
                                    services.AddScoped<INotificationService<TweetStatistics>, TweetConsoleNotificationService>();

                                    services.AddLogging(c =>
                                    {
                                        c.ClearProviders();
                                        c.AddSerilog(logger);
                                    });
                                });
        }
    }
}