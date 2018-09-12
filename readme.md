# Configuration guide

This template requires two levels of configuration 

- parameters for template execution
- parameters to set after the content has been generated - there are several reasons for these - limitations of current .net CLI and its ability to parse/process complex values, their business value (they are likely not known at the initial stages of the project) and also their overall number (and the absence of defaults) would make the
items to configure

Template recognizes two required params

- APIName - name of the API e.g. Fraud.API
- APIPortNumber - port number e.g. 15000

The other (post generation) attributes are

- swagger version (see Startup.cs in the API project)
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

# Generate content

after the template is installed, run the following:

``` shell
mkdir {put solution name here}
cd {put solution name here}
dotnet new ESWWebAPI --APIName {put API name here} --APIPortNumber {put port here}
```

# Building it locally

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