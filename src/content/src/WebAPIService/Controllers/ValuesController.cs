using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APINamespace.Controllers
{
    /// <summary>
    /// sample controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ValuesController : Controller
    {
        /// <summary>
        /// GET implementation for default route
        /// </summary>
        /// <returns>see response code to response type metadata, list of all values</returns>
        /// <response code="200">List of all values</response>
        /// <response code="401">Caller is Unauthorized</response>
        [HttpGet]
        [ProducesResponseType(typeof(string[]), (int)HttpStatusCode.OK)]
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
        /// <response code="200">individual value for a given id</response>
        /// <response code="401">Caller is Unauthorized</response>
        /// <response code="404">No entry exists for the given id</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get(int id)
        {
            return await Task.FromResult(new JsonResult("value"));
        }

        /// <summary>
        /// post
        /// </summary>
        /// <param name="value">payload</param>
        /// <returns>action result</returns>
        /// <response code="201">A new value was created</response>
        /// <response code="400">The request is malformed</response>
        /// <response code="401">Caller is Unauthorized</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody]string value)
        {

            if (string.IsNullOrWhiteSpace(value))
                return await Task.FromResult(BadRequest());
            else
                return await Task.FromResult(CreatedAtAction("POST", new { id = value }, value));
        }

        /// <summary>
        /// Put
        /// </summary>
        /// <param name="id">id to process</param>
        /// <param name="value">payload</param>
        /// <returns>action result</returns>
        /// <response code="200">The put operation was successful</response>
        /// <response code="400">The request is malformed</response>
        /// <response code="401">Caller is Unauthorized</response>
        /// <response code="404">No entry exists for the given id</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        /// <response code="200">The delete operation was successful</response>
        /// <response code="401">Caller is Unauthorized</response>
        /// <response code="404">No entry exists for the given id</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            return await Task.FromResult(NoContent());
        }
    }
}
