# WEBAPIService Guide

# Summary

# Controller and API Versoning

ASPNet Core provides API versionings conventions in multiple ways. For design and operational reasons versoning is part of the URI contract, eg: http://localhost/api/v1/values

In this case when creating multiple versions, this should follow the following convention:

```
http://localhost/api/v1/values
http://localhost/api/v2/values
```

In code for a version 1 (v1) controller this looks like the following:

```c# 
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(string[]), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(new JsonResult(new[] { "value1", "value2" }));
        }
	}
```

For a version 2 (v2) controller this looks like the following:

```c#
    [Produces("application/json")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(string[]), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(new JsonResult(new[] { "value1", "value2" }));
        }
	}
```