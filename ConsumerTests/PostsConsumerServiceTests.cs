using System.Reflection;
using EventBus;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using PostConsumerService;
using Xunit;

namespace ConsumerTests
{
    public class PostsConsumerServiceTests
    {
        private readonly Mock<ILogger<PostsConsumerService>> mockLogger;
        private readonly Mock<IEventBus> mockEventBus;
        private readonly PostsConsumerService postsConsumerService;
        private readonly RedditPostDataStore redditPostDataStore;

        public PostsConsumerServiceTests()
        {
            mockLogger = new Mock<ILogger<PostsConsumerService>>();
            mockEventBus = new Mock<IEventBus>();
            redditPostDataStore = new RedditPostDataStore();
            postsConsumerService = new PostsConsumerService(mockLogger.Object, redditPostDataStore, mockEventBus.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ProcessesPostReceivedEvent_CorrectlyUpdatesDataStore()
        {
            // Arrange
            Post post = new Post() { Id = "Post 1", Ups = 20, Name = "John" };
            PostEvent mockPostReceivedEvent = new PostReceivedEvent(post);

            mockEventBus.SetupSequence(bus => bus.TryDequeue(out mockPostReceivedEvent))
  .Returns(true);

            // Act
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(5000);
            var cancellationToken = cancellationTokenSource.Token;
            MethodInfo executeAsyncMethod = typeof(PostsConsumerService).GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            try
            {
                executeAsyncMethod.Invoke(postsConsumerService, new object[] { cancellationToken });
            }
            catch (TaskCanceledException)
            {
                // Task is cancelled, so this exception is expected
            }

            // Assert
            // Verify that the data store has been updated correctly based on the mock PostReceivedEvent
            Assert.Equal(1, redditPostDataStore.topPosts.Count);
            Assert.Equal(1, redditPostDataStore.topUsers.Count);
        }
    }
}