using System.Text.Json;
using EventBus;
using HttpSideCar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;

namespace PostPublisherService
{
    public class PostsPublisherService : BackgroundService
    {
        private readonly ILogger<PostsPublisherService> logger;
        private readonly RateLimitingService _rateLimitingService;
        private readonly IEventBus eventBus;
        private readonly RedditApiCredentials credentials;

        public PostsPublisherService(ILogger<PostsPublisherService> logger, RateLimitingService rateLimitingService, IEventBus eventBus, IOptions<RedditApiCredentials> credentialsOptions)
        {
            this.logger = logger;
            this._rateLimitingService = rateLimitingService;
            this.eventBus = eventBus;
            this.credentials = credentialsOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string? after = String.Empty;
            while (!stoppingToken.IsCancellationRequested) 
            {
                try 
                {
                    var url = this.credentials.GetSubRedditPostUrl;
                    if (!string.IsNullOrEmpty(after))
                    {
                        url = $"{url}?after={after}";
                    }

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    var response = await _rateLimitingService.SendRequest(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var redditResponse = !string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject<RedditResponse>(content) : null;
                        var postList = redditResponse?.Data?.Children;

                        after = redditResponse?.Data?.After;

                        //Push messages to In Memory Event Bus
                        if (postList != null && postList.Any())
                        {
                            foreach (var item in postList)
                            {
                                eventBus.Enqueue(new PostReceivedEvent(item.Data));
                            };
                        }
                    }
                } catch (Exception e)
                {
                    logger.LogError(e.Message);
                    throw;
                }
            }
        }
    }
}