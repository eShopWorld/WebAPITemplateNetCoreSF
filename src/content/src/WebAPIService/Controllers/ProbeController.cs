using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIService.Controllers
{
    /// <summary>
    /// health probe controller
    /// </summary>
    [Produces("application/json")]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ProbeController : Controller
    {
        /// <summary>
        /// empty get to serve as health probe endpoint
        /// </summary>
        [HttpGet]
        public void Get()
        {
            
        }
    }
}