using JackHenryChallenge.ServiceAgents;
using JackHenryChallenge.ServiceAgents.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Moq.Protected;
using Serilog;
using Serilog.Core;
using System.Net;
using System.Text;
using Xunit;
namespace JackHenryChallenge.IntegrationTests
{
    public class UnitTest1
    {

        [Fact]
        public async Task Test_HappyPath()
        {
            const string data1 = @"{""data"":{""entities"":{""hashtags"":[{""start"":16,""end"":34,""tag"":""Tag1""}],""mentions"":[{""start"":3,""end"":12,""username"":""mintsoo3"",""id"":""1349731183509164032""}],""urls"":[{""start"":34,""end"":57,""url"":""https://t.co/QqmTEX7yi0"",""expanded_url"":""https://twitter.com/mintsoo3/status/1559650904999018496/video/1"",""display_url"":""pic.twitter.com/QqmTEX7yi0"",""media_key"":""7_1559650875055910912""}]},""id"":""1559758779742564352"",""text"":""RT @mintsoo3: ??????\n?? D-3? ???? https://t.co/QqmTEX7yi0""}}";
            const string data2 = @"{""data"":{""entities"":{""hashtags"":[{""start"":16,""end"":34,""tag"":""Tag1""},{""start"":16,""end"":34,""tag"":""Tag2""}],""mentions"":[{""start"":3,""end"":14,""username"":""destinyzee"",""id"":""593806156""}],""urls"":[{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586557664837634""},{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586569333477377""},{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586587457081352""},{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586602938241025""}]},""id"":""1559758779721388036"",""text"":""RT @destinyzee: #IncaseYouMissedIt niyabora yoh?? https://t.co/PLPWiukniH""}}";

            var testData = $@"{data1}
{data2}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(stream)
                    });
                using (var logger = new LoggerConfiguration().CreateLogger())
                using (var client = new HttpClient(mockMessageHandler.Object))
                using (var host = CreateHostBuilder(logger, client).Build())
                using (StringWriter sw = new StringWriter())
                {
                    client.BaseAddress = new Uri("https://api.twitter.com");
                    Console.SetOut(sw);
                    await host.RunAsync();
                    var actual = sw.ToString();

                    string expected = String.Format("Tweet Count:2, Top 10 hashtags:Tag1,Tag2{0}", Environment.NewLine);
                    Assert.Equal(expected, actual);
                }
            }
        }

        internal static IHostBuilder CreateHostBuilder(Logger logger, HttpClient client)
        {
            var hostBuilder = Program.CreateHostBuilder(logger);
            hostBuilder.ConfigureServices((context, services) =>
            {
                var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(HttpClient));
#pragma warning disable CS8604 // Possible null reference argument.
                services.Remove(serviceDescriptor);
                serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ITwitterSa));
                services.Remove(serviceDescriptor);
#pragma warning restore CS8604 // Possible null reference argument.
                services.AddScoped<ITwitterSa, TwitterSa>();
                services.AddSingleton<HttpClient>(client);
            });
            return hostBuilder;



        }

    }

}