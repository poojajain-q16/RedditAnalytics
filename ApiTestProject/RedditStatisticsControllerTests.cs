using BusinessLogic.Contracts;
using Microsoft.AspNetCore.Mvc;
using Models.ResponseModels;
using Moq;
using RedditAPIStatistics.Controllers.v1;
using Xunit;
using Microsoft.Extensions.Logging;

namespace ApiTestProject
{
    public class RedditStatisticsControllerTests
    {

        [Fact]
        public void GetPostStatistics_ReturnsOkWithStatisticsResponseModel()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RedditStatisticsController>>();
            var mockService = new Mock<IRedditStatisticsService>();

            // Create a sample StatisticsResponseModel for the mockService
            var mockResponse = new StatisticsResponseModel
            {
                // Set the properties as per your actual implementation
                PostsWithMostUpVotes = new List<PostResponseModel>()
                {
                    new PostResponseModel() { Id = "123", Title = "Today is a good day.", UpCount = 27 },
                    new PostResponseModel() { Id = "223", Title = "Today is a sunny day.", UpCount = 56 }
                },
                UsersWithMostPosts = new List<UserResponseModel>()
                {
                    new UserResponseModel() { PostCount = 10, User = "Adam"},
                    new UserResponseModel() { PostCount = 6, User = "Michelle"},
                    new UserResponseModel() { PostCount = 9, User = "John"}
                }
                // Add other properties as needed
            };

            // Setup the mockService to return the sample response
            mockService.Setup(service => service.GetPostStatistics()).Returns(mockResponse);

            var controller = new RedditStatisticsController(mockLogger.Object, mockService.Object);

            // Act
            var result = controller.GetPostStatistics();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseModel = Assert.IsType<StatisticsResponseModel>(okResult.Value);

            // Assert the values of the StatisticsResponseModel if needed
            Assert.Equal(2, responseModel.PostsWithMostUpVotes.Count);
            Assert.Equal(27, responseModel.PostsWithMostUpVotes.First().UpCount);
            Assert.Equal(3, responseModel.UsersWithMostPosts.Count);
        }
    }
}