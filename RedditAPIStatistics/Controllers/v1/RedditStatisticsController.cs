using BusinessLogic.Contracts;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ResponseModels;
using System.Net;

namespace RedditAPIStatistics.Controllers.v1
{
    [Route("api/v1/reddit/statistics/")]
    [ApiController]
    public class RedditStatisticsController : BaseController
    {
        private readonly ILogger<RedditStatisticsController> logger;
        private readonly IRedditStatisticsService redditStatisticsService;

        public RedditStatisticsController(ILogger<RedditStatisticsController> logger, IRedditStatisticsService redditStatisticsService)
        {
            this.logger = logger;
            this.redditStatisticsService = redditStatisticsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(StatisticsResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MessageStatusModel), (int)HttpStatusCode.NotFound)]
        public IActionResult GetPostStatistics()
        {
            logger.LogDebug("Begin: GetPostStatistics");
            var response = redditStatisticsService.GetPostStatistics();
            logger.LogDebug("End: GetPostStatistics");

            return Ok(response);
        }
    }
}

