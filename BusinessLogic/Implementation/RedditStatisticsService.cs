using BusinessLogic.Contracts;
using Microsoft.Extensions.Logging;
using Models;
using Models.ResponseModels;
using AutoMapper;

namespace BusinessLogic.Implementation
{
    public class RedditStatisticsService : IRedditStatisticsService
    {
        private readonly ILogger<RedditStatisticsService> logger;
        private readonly RedditPostDataStore dataStore;
        private readonly IMapper mapper;

        public RedditStatisticsService(ILogger<RedditStatisticsService> logger, RedditPostDataStore dataStore, IMapper mapper)
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.mapper = mapper;
            
        }

        /// <summary>
        /// This method fetches the data from data store.
        /// </summary>
        /// <returns>ResponseModel having Top Posts with most ups and users with most posts.</returns>
        public StatisticsResponseModel GetPostStatistics()
        {
            this.logger.LogDebug($"Begin: GetPostStatistics");
            var topPosts = dataStore.GetTop5PostsWithMostUpvotes();
            var userWithMostPosts = dataStore.GetUserWithMostPosts();

            return new StatisticsResponseModel
            {
                PostsWithMostUpVotes = this.mapper.Map<List<PostResponseModel>>(topPosts),
                UsersWithMostPosts = this.mapper.Map<List<UserResponseModel>>(userWithMostPosts)
            };
        }
    }
}
