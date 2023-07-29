using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Net;

namespace RedditAPIStatistics.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        protected ActionResult ErrorResponse(string message, HttpStatusCode code)
        {
            MessageStatusModel messageStatusModel = new() { ResponseCode = code.ToString(), Description = message };
            return this.StatusCode((int)code, messageStatusModel);
        }
    }
}
