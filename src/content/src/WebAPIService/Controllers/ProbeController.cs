using Microsoft.AspNetCore.Mvc;

namespace WebAPIService.Controllers
{
    /// <summary>
    /// health probe controller
    /// </summary>
    ///
    [ApiVersionNeutral]
    [Route("[controller]")]
    [Produces("application/json")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class ProbeController : Controller
    {
        /// <summary>
        /// empty get to serve as health probe endpoint
        /// </summary>
        [HttpGet]
        public StatusCodeResult Get()
        {
            return StatusCode(200);
        }
    }
}