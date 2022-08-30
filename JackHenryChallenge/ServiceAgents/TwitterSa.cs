using JackHenryChallenge.ServiceAgents.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using JackHenryChallenge.Models;
using JackHenryChallenge.Models.Twitter;

namespace JackHenryChallenge.ServiceAgents
{
    /// <summary>
    /// Twitter service agent that talks with the twitter service
    /// </summary>
    /// <seealso cref="Interfaces.ITwitterSa" />
    public class TwitterSa : ITwitterSa
    {
        private readonly ILogger<TwitterSa> _logger;
        private readonly HttpClient _httpClient;
        private const string ContentTypeJson = "application/json";
        private static readonly JsonSerializerOptions StreamJsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterSa"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// httpClient
        /// or
        /// logger
        /// </exception>
        public TwitterSa(HttpClient httpClient, ILogger<TwitterSa> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    
        /// <summary>
        /// Gets the twitter sample stream.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async IAsyncEnumerable<TwitterResponse<Tweet>> GetSampleStream(GetSampleStreamRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            string url = QueryHelpers.AddQueryString(request.Url, request.QueryStrings);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypeJson));
                var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var streamReader = new StreamReader(stream))
                    {
                        while (!streamReader.EndOfStream && cancellationToken.IsCancellationRequested == false)
                        {
                            var currentLine = await streamReader.ReadLineAsync().WaitAsync(cancellationToken);
                            TwitterResponse<Tweet>? tweetData = null;
                            try
                            {
                                _logger.LogDebug("Twitter Sa Current Line:{streamLine}", currentLine);
                                tweetData = string.IsNullOrEmpty(currentLine) ?
                                    null : JsonSerializer.Deserialize<TwitterResponse<Tweet>>(currentLine, StreamJsonOptions);
                            }
                            catch (JsonException ex)
                            {
                                _logger.LogError(ex, "Could not process json:{streamLine}", currentLine);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Could not process line:{streamLine}", currentLine);
                                throw;
                            }
                            if (tweetData != null)
                            {
                                yield return tweetData;
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError("Failed to succesfully get a response,statusCode:{statusCode},uri:{uri}", response.StatusCode, requestMessage.RequestUri);
                }
            }

            yield break;
        }

        //public async IAsyncEnumerable<TweetData> GetSampleStream(
        //    [EnumeratorCancellation] CancellationToken cancellationToken = default)
        //{
        //    var url = "/2/tweets/sample/stream?tweet.fields=entities";

        //    using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
        //    {
        //        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypeJson));
        //        var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        //        if (response != null && response.IsSuccessStatusCode)
        //        {
        //            using (var stream = await response.Content.ReadAsStreamAsync())
        //            using (var streamReader = new StreamReader(stream))
        //            {
        //                while (!streamReader.EndOfStream && cancellationToken.IsCancellationRequested == false)
        //                {
        //                    var currentLine = await streamReader.ReadLineAsync().WaitAsync(cancellationToken);
        //                    TweetData? tweetData = null;
        //                    try
        //                    {
        //                        _logger.LogDebug("Twitter Sa Current Line:{streamLine}", currentLine);
        //                        tweetData = string.IsNullOrEmpty(currentLine) ?
        //                            null : JsonSerializer.Deserialize<TweetData>(currentLine, StreamJsonOptions);
        //                    }
        //                    catch (JsonException ex)
        //                    {
        //                        _logger.LogError(ex, "Could not process json:{streamLine}", currentLine);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _logger.LogError(ex, "Could not process line:{streamLine}", currentLine);
        //                        throw;
        //                    }
        //                    if (tweetData != null)
        //                    {
        //                        yield return tweetData;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    yield break;
        //}    
    }
}
