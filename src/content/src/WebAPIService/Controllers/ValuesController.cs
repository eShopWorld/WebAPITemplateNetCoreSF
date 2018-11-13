using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIService.Controllers
{
    /// <summary>
    /// sample controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ValuesController : Controller
    {
        /// <summary>
        /// GET implementation for default route
        /// </summary>
        /// <returns>see response code to response type metadata, list of all values</returns>
        [HttpGet]
        [ProducesResponseType(typeof(string[]), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(new JsonResult(new[] { "value1", "value2" }));
        }

        /// <summary>
        /// Get with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>see response code to response type metadata, individual value for a given id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get(int id) => await Task.FromResult(new JsonResult("value"));

        /// <summary>
        /// post
        /// </summary>
        /// <param name="value">payload</param>
        /// <returns>action result</returns>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return await Task.FromResult(BadRequest());
            else
                return await Task.FromResult(Ok());
        }

        /// <summary>
        /// Put
        /// </summary>
        /// <param name="id">id to process</param>
        /// <param name="value">payload</param>
        /// <returns>action result</returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put(int id, [FromBody]string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return await Task.FromResult(BadRequest());
            else
                return await Task.FromResult(Ok());
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">id to delete</param>
        /// <returns>action result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            return await Task.FromResult(Ok());
        }
    }
}
