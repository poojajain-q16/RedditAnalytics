using System.Net;
using System.Reflection;
using EventBus;
using HttpSideCar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using Newtonsoft.Json;
using PostPublisherService;
using Xunit;

namespace PublisherTests
{
    public class PostsPublisherServiceTests
    {
        private readonly Mock<ILogger<PostsPublisherService>> mockLogger;
        private readonly Mock<IEventBus> mockEventBus;
        private readonly Mock<IOptions<RedditApiCredentials>> mockOptions;
        private readonly Mock<AuthenticationService> mockAuthService;
        private readonly Mock<RateLimitingService> mockRateLimitingService;
        private readonly PostsPublisherService postsPublisherService;

        public PostsPublisherServiceTests()
        {
            mockLogger = new Mock<ILogger<PostsPublisherService>>();
            mockEventBus = new Mock<IEventBus>();

            var mockCredentials = new RedditApiCredentials
            {
                GetSubRedditPostUrl = "https://www.reddit.com/r/testreddit/new.json" // Provide the actual URL
            };
            mockOptions = new Mock<IOptions<RedditApiCredentials>>();
            mockOptions.Setup(opt => opt.Value).Returns(mockCredentials);

            mockAuthService = new Mock<AuthenticationService>(mockOptions.Object);
            mockRateLimitingService = new Mock<RateLimitingService>(mockAuthService.Object);

            postsPublisherService = new PostsPublisherService(mockLogger.Object, mockRateLimitingService.Object, mockEventBus.Object, mockOptions.Object);
        }

        [Fact]
        public async Task ExecuteAsync_SuccessfullyEnqueuesPostReceivedEvents()
        {
            // Arrange
            mockRateLimitingService.Setup(service => service.SendRequest(It.IsAny<HttpRequestMessage>()))
              .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(GetMockRedditResponse()) });

            // Act
            // Create a CancellationTokenSource to get the CancellationToken
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(1000);
            var cancellationToken = cancellationTokenSource.Token;

            MethodInfo executeAsyncMethod = typeof(PostsPublisherService).GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            var executeAsyncTask = (Task)executeAsyncMethod.Invoke(postsPublisherService, new object[] { cancellationToken });

            // Check if the test completed successfully or if it timed out
            if (executeAsyncTask.IsCompletedSuccessfully)
            {
                // Assert
                // Verify that the event bus is correctly enqueued with PostReceivedEvents
                mockEventBus.Verify(bus => bus.Enqueue(It.IsAny<PostReceivedEvent>()), Times.AtLeast(3));
            }
            else
            {
                // Test timed out
                cancellationTokenSource.Cancel();
                Assert.True(false, "Test timed out.");
            }
        }

        private static string GetMockRedditResponse()
        {
            // Mock Reddit API response JSON string here for testing purposes.
            var redditResponse = new RedditResponse
            {
                Data = new ChildPosts()
                {
                    Children = new List<PostData>() {
                    new PostData() { Data = new Post() { Id = "post1", Title = "Post 1", Ups = 100 } } ,
                    new PostData() { Data = new Post() { Id = "post2", Title = "Post 2", Ups = 90 } },
                    new PostData() { Data = new Post() { Id = "post3", Title = "Post 3", Ups = 267 } }
                }
                }
            };

            return JsonConvert.SerializeObject(redditResponse);
        }
    }
}