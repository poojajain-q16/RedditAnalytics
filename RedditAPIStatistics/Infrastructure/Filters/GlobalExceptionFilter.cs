using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using System.Net;

namespace RedditAPIStatistics.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, "Error caught by GlobalExceptionFilter");
            var messageStatus = new MessageStatusModel
            {
                ResponseCode = $"{(int)HttpStatusCode.InternalServerError}",
                Description = "Internal Server Error",
            };

            context.Result = new ObjectResult(messageStatus);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
