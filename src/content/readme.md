# Configuration guide

This template requires two levels of configuration 

- parameters for template execution.
- parameters to set after the content has been generated<sup>1</sup>.

Template recognizes three required params

- APIName - name of the API e.g. Fraud.API
- APIPortNumber - port number e.g. 15000
- APIHttpsPortNumber - port number for HTTPS traffic e.g. 15001 (must be different than APIPortNumber)

The other (post generation) attributes are

- swagger version (see Startup.cs in the API project)
- OpenApi specification version (version 3 not yet supported by Autorest) - see Startup.cs and the UseOpenApiV2 flag - this mode is enabled by default
- version prefix in swagger endpoint (also in Startup.cs)
- port in service manifest (see ServiceManifest.xml in API project)
- AppInsights instrumentation key for the app and the internal one for BigBrother (see appsettings json file corresponding to the target environment (e.g appsettings.Production.json) -see "TBA" values
 - STS configuration - see appsettings json corresponding to the environment - see ServiceConfigurationOptions node and values of "TBA"

# Installation guide

To install (and uninstall) the template please follow https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new?tabs=netcore2x. Pay particular attention to -i and -u switches

```shell

dotnet new -i Eshopworld.WebAPIFabric.Template (takes it from nuget)

```

e.g dotnet new -i Eshopworld.WebAPIFabric.Template (or download the nuget package and then use the path to it) - this package is available at github-dev feed of eshopworld.myget.org

## Generate a new WebAPI from the Template

after the template is installed, run the following:

``` shell
mkdir {put solution name here}
cd {put solution name here}
dotnet new ESWWebAPI --APIName {put API name here} --APIPortNumber {put port here}
```

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

# Building the template locally

1. Update the template as needed.
1. Update the nuspec file.
1. Build the nuget package (you need nuget.exe for this)
1. Then install it locally with the below cli and path:


Install the template locally
```shell

dotnet new -i [path to the dir of the .template.config file]

```

Uninstall the template locally

```shell

dotnet new -u [path to the dir of the .template.config file]

```


<sup>1</sup> There are several reasons for setting parameters after the fact: 
1. Limitations of current .net CLI
1. It's ability to parse/process complex values.
1. Their business value (they are likely not known at the initial stages of the project)1. Their overall number (and the absence of defaults).