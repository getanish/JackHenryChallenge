using JackHenryChallenge.Models.Twitter;
using JackHenryChallenge.ServiceAgents;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;
using JackHenryChallenge.Models;

namespace JackHenryChallenge.Tests.ServiceAgents
{
    public class TwitterSaTests
    {
        const string data1 = @"{""data"":{""entities"":{""mentions"":[{""start"":3,""end"":12,""username"":""mintsoo3"",""id"":""1349731183509164032""}],""urls"":[{""start"":34,""end"":57,""url"":""https://t.co/QqmTEX7yi0"",""expanded_url"":""https://twitter.com/mintsoo3/status/1559650904999018496/video/1"",""display_url"":""pic.twitter.com/QqmTEX7yi0"",""media_key"":""7_1559650875055910912""}]},""id"":""1559758779742564352"",""text"":""RT @mintsoo3: ??????\n?? D-3? ???? https://t.co/QqmTEX7yi0""}}";
        const string data2 = @"{""data"":{""entities"":{""annotations"":[{""start"":136,""end"":138,""probability"":0.9609,""type"":""Place"",""normalized_text"":""USA""}],""urls"":[{""start"":141,""end"":164,""url"":""https://t.co/UvJFlCELUa"",""expanded_url"":""https://twitter.com/repswalwell/status/1557428889902895104"",""display_url"":""twitter.com/repswalwell/st.""}]},""id"":""1559758779750793222"",""text"":""This is beyond disturbing! Threatening to cut off even children's heads because their parent is cancellationTokenSource democrat?! This is what's ruining the USA! https://t.co/UvJFlCELUa""}}";
        const string data3 = @"{""data"":{""entities"":{""hashtags"":[{""start"":16,""end"":34,""tag"":""IncaseYouMissedIt""}],""mentions"":[{""start"":3,""end"":14,""username"":""destinyzee"",""id"":""593806156""}],""urls"":[{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586557664837634""},{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586569333477377""},{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586587457081352""},{""start"":49,""end"":72,""url"":""https://t.co/PLPWiukniH"",""expanded_url"":""https://twitter.com/destinyzee/status/1559586615332413443/photo/1"",""display_url"":""pic.twitter.com/PLPWiukniH"",""media_key"":""3_1559586602938241025""}]},""id"":""1559758779721388036"",""text"":""RT @destinyzee: #IncaseYouMissedIt niyabora yoh?? https://t.co/PLPWiukniH""}}";
        const string invalidData = @"{""data"":start"":49,""end"":72,";

        [Fact]
        public async Task GetSampleStream_ContinuesOnJsonException()
        {
            var testData = $@"{invalidData}
{data1}
{data2}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var mockLogger = new Mock<ILogger<TwitterSa>>();
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(stream)
                    });
                using (var client = new HttpClient(mockMessageHandler.Object))
                {
                    client.BaseAddress = new Uri("https://www.google.com/");
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var sa = new TwitterSa(client, mockLogger.Object);
                    var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });
                    var result = sa.GetSampleStream(request, CancellationToken.None);
                    var ct = await result.CountAsync();
                    Assert.Equal(2, ct);


                }


            }

        }

        [Fact]
        public async Task GetSampleStream_ReturnsForUnsuccessfullCall()
        {
            var testData = $@"{data1}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var mockLogger = new Mock<ILogger<TwitterSa>>();
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StreamContent(stream)
                    });
                using (var client = new HttpClient(mockMessageHandler.Object))
                {
                    client.BaseAddress = new Uri("https://www.google.com/");
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var sa = new TwitterSa(client, mockLogger.Object);
                    var result = new ConcurrentBag<TwitterResponse<Tweet>>();
                    var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });
                    await foreach (var item in sa.GetSampleStream(request, CancellationToken.None))
                    {
                        result.Add(item);
                    }

                    Assert.Empty(result);
                }


            }
        }

        [Fact]
        public async Task GetSampleStream_ReturnsTweetsandTags()
        {
            var testData = $@"{data1}
{data2}
{data3}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var mockLogger = new Mock<ILogger<TwitterSa>>();
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(stream)
                    });
                using (var client = new HttpClient(mockMessageHandler.Object))
                {
                    client.BaseAddress = new Uri("https://www.google.com/");
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var sa = new TwitterSa(client, mockLogger.Object);
                    var result = new ConcurrentBag<TwitterResponse<Tweet>>();
                    var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });
                    await foreach (var item in sa.GetSampleStream(request, CancellationToken.None))
                    {
                        result.Add(item);
                    }

                    Assert.NotNull(result.FirstOrDefault(verifyData1));
                    Assert.NotNull(result.FirstOrDefault(verifyData2));
                    Assert.NotNull(result.FirstOrDefault(verifyData3));
                    Assert.Equal(3, result.Count());
                }


            }
            bool verifyData1(TwitterResponse<Tweet> tweetData) =>
                            verifyData(tweetData.Data, "1559758779742564352", "RT @mintsoo3: ??????\n?? D-3? ???? https://t.co/QqmTEX7yi0", 0, null);
            bool verifyData2(TwitterResponse<Tweet> tweetData) =>
                            verifyData(tweetData.Data, "1559758779750793222", "This is beyond disturbing! Threatening to cut off even children's heads because their parent is cancellationTokenSource democrat?! This is what's ruining the USA! https://t.co/UvJFlCELUa", 0, null);
            bool verifyData3(TwitterResponse<Tweet> tweetData) =>
                            verifyData(tweetData.Data, "1559758779721388036", "RT @destinyzee: #IncaseYouMissedIt niyabora yoh?? https://t.co/PLPWiukniH", 1, new[] { "IncaseYouMissedIt" } );

            


        }

        private static bool verifyData(Tweet tweet, string id, string text, int hashTagCount, string[]? hashTags)
        {
            var condition = tweet.Id.Equals(id) &&
                        tweet.Text.Equals(text);
            if (condition && hashTagCount > 0)
            {
                condition &= hashTagCount == tweet.Entities?.Hashtags?.Length;
                if (condition && hashTags != null)
                {
                    foreach (var tag in hashTags)
                    {
                        condition &= tweet.Entities?.Hashtags?.FirstOrDefault(x => x.Tag.Equals(tag)) != null;
                    }
                }

            }
            return condition;
        }

        [Fact]
        public async Task GetSampleStream_IgnoresEmptyLines()
        {
            var testData = $@"{data1}

{data2}
{data3}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var mockLogger = new Mock<ILogger<TwitterSa>>();
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(stream)
                    });
                using (var client = new HttpClient(mockMessageHandler.Object))
                {
                    client.BaseAddress = new Uri("https://www.google.com/");
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var sa = new TwitterSa(client, mockLogger.Object);
                    var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });
                    var result = sa.GetSampleStream(request, CancellationToken.None);
                    Assert.NotNull(result);
                    var ct = await result.CountAsync();
                    Assert.Equal(3, ct);
                }


            }

        }

        [Fact]
        public async Task GetSampleStream_VerifyMediaType()
        {
            var testData = $@"{data1}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var mockLogger = new Mock<ILogger<TwitterSa>>();
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(stream)
                    });
                using (var client = new HttpClient(mockMessageHandler.Object))
                {
                    client.BaseAddress = new Uri("https://www.google.com/");
                    var sa = new TwitterSa(client, mockLogger.Object);
                    var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });

                    var result = sa.GetSampleStream(request, CancellationToken.None);
                    Assert.NotNull(result);
                    var ct = await result.CountAsync();
                }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                mockMessageHandler.Protected()
                    .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(
                        x => x.Headers.Accept.FirstOrDefault(x => x.MediaType.Equals("application/json", StringComparison.Ordinal)) != null), ItExpr.IsAny<CancellationToken>());
#pragma warning restore CS8602 // Dereference of a possibly null reference.



            }


        }

        [Fact]
        public async Task GetSampleStream_RespectsCancellationToken()
        {
            using (var stream = new MemoryStream())
            using(var writer = new StreamWriter(stream))
            {
                var mockLogger = new Mock<ILogger<TwitterSa>>();
                var mockMessageHandler = new Mock<HttpMessageHandler>();
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(stream)
                    });
                using (var client = new HttpClient(mockMessageHandler.Object))
                {
                    client.BaseAddress = new Uri("https://www.google.com/");
                    var sa = new TwitterSa(client, mockLogger.Object);
                    var result = new ConcurrentBag<TwitterResponse<Tweet>>();
                    var request = new GetSampleStreamRequest(new List<Field> { Field.Entities });
                    var cancellationTokenSource = new CancellationTokenSource();

                    writer.WriteLine(data1);
                    writer.WriteLine(data2);
                    writer.WriteLine(data3);
                    writer.Flush();
                    stream.Position = 0;
                    var ct = 0;
                    await foreach (var item in sa.GetSampleStream(request, cancellationTokenSource.Token))
                    {
                        Interlocked.Increment(ref ct);
                        if (ct > 1)
                        {
                            cancellationTokenSource.Cancel();
                        }
                    }
                    Assert.Equal(2,ct);
                }
            }
        }

        [Fact]
        public void TwitterSa_RaisesExceptionOnNullClient()
        {
            var mockLogger = new Mock<ILogger<TwitterSa>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var exception = Assert.Throws<ArgumentNullException>(() => new TwitterSa(null, mockLogger.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Equal("httpClient", exception.ParamName);
            
        }

        [Fact]
        public void TwitterSa_RaisesExceptionOnNullLogger()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var exception = Assert.Throws<ArgumentNullException>(() => new TwitterSa(new HttpClient(), null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Equal("logger", exception.ParamName);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
