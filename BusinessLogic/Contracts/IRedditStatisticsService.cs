using Models.ResponseModels;

namespace BusinessLogic.Contracts
{
    public interface IRedditStatisticsService
    {
        public StatisticsResponseModel GetPostStatistics();
    }
}
