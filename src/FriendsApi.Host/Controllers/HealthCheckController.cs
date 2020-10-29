using Microsoft.AspNetCore.Mvc;
using FriendsApi.Host.Constants;

namespace FriendsApi.Host.Controllers
{
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Healthcheck
        /// </summary>
        /// <remarks>Healthcheck</remarks>
        /// <response code="200">Return status api</response>
        [HttpGet]
        [Route(Routes.HealthCheck)]
        public ActionResult Get()
        {
            return Ok();
        }
    }
}