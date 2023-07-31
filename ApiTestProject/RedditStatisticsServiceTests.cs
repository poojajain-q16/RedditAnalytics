using AutoMapper;
using BusinessLogic.Implementation;
using Microsoft.Extensions.Logging;
using Models.ResponseModels;
using Models;
using Moq;
using Xunit;

namespace ApiTestProject
{
    public class RedditStatisticsServiceTests
    {
        [Fact]
        public void GetPostStatistics_ReturnsCorrectStatisticsResponseModel()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RedditStatisticsService>>();
            var dataStore = new RedditPostDataStore();
            var mockMapper = new Mock<IMapper>();

            // Create sample data for the data store results
            dataStore.topPosts = new SortedSet<Post>(Comparer<Post>.Create((x, y) => x.Ups.CompareTo(y.Ups)))
            {
                new Post { Id = "1", Title = "Post 1", Ups = 100 },
                new Post { Id = "2", Title = "Post 2", Ups = 90 },
                new Post { Id = "3", Title = "Post 3", Ups = 34 },
                new Post { Id = "4", Title = "Post 4", Ups = 123 },
                new Post { Id = "5", Title = "Post 5", Ups = 332 }
            };

            dataStore.topUsers = new SortedSet<UserPostCount>(Comparer<UserPostCount>.Create((x, y) => x.PostCount.CompareTo(y.PostCount)))
            {
                new UserPostCount { User = "User A", PostCount = 10 },
                new UserPostCount { User = "User B", PostCount = 8 },
            };

            // Create sample response models for mapping
            var samplePostResponseModels = new List<PostResponseModel>
            {
                new PostResponseModel { Id = "5", Title = "Post 5", UpCount = 332 },
                new PostResponseModel { Id = "4", Title = "Post 4", UpCount = 123 },
                new PostResponseModel { Id = "1", Title = "Post 1", UpCount = 100 },
                new PostResponseModel { Id = "2", Title = "Post 2", UpCount = 90 },
                new PostResponseModel { Id = "3", Title = "Post 3", UpCount = 34 }
            };

            var sampleUserResponseModels = new List<UserResponseModel>
            {
                new UserResponseModel { User = "User A", PostCount = 10 },
                new UserResponseModel { User = "User B", PostCount = 8 },
              };

            // Setup the mapper mock to return the sample response models
            mockMapper.Setup(mapper => mapper.Map<List<PostResponseModel>>(dataStore.topPosts.Reverse()))
        .Returns(samplePostResponseModels);

            mockMapper.Setup(mapper => mapper.Map<List<UserResponseModel>>(dataStore.topUsers.Reverse()))
              .Returns(sampleUserResponseModels);

            var redditStatisticsService = new RedditStatisticsService(mockLogger.Object, dataStore, mockMapper.Object);

            // Act
            var result = redditStatisticsService.GetPostStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(samplePostResponseModels, result.PostsWithMostUpVotes);
            Assert.Equal(sampleUserResponseModels, result.UsersWithMostPosts);
        }
    }
}
