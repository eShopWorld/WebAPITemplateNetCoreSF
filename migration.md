# Migration Guide

## Prerequisites 

* Visual Studio 2019
* .Net Core SDK
* Latest Service Fabric SDK and Runtime

## Migration

The following migration guide covers the migration from a previous version of the EShopWorld Web Api Service Fabric template to the current recommended version of the .Net Core framework and AspNetCore runtime.

What is not covered are any additional frameworks or customisations that have been applied to an instance of a Web Api that was generated from a previous version of this template.

The current version of the template is base on .Net Core 3.1 AspNetCore 3.1, the previous release was based on .Net Core 2.2 AspNetCore 2.2

There are a number of modifications required in the migration from .net core 2.2 AspNetCore 2.2 to Core 3.1 AspNetCore 3.1.

 From | To Version | Migration
---------|----------|---------
 2.2 | 3.0 | https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-3.1&tabs=visual-studioC1
 3.0 | 3.1 | https://docs.microsoft.com/en-us/aspnet/core/migration/30-to-31?view=aspnetcore-3.1&tabs=visual-studio

This migration guide consolidates the stages outline in the documents above. Please refer to the above links for more detailed information around breaking behavioural changes.

## Update Core Framework Version

Update the target framework version

``` xml
<TargetFramework>netcoreapp2.2</TargetFramework>
```

``` xml
<TargetFramework>netcoreapp3.1</TargetFramework>
```

## Update References

A number of packages are no longer available for AspNetCore 3.1 and should be removed from the proj file.

See the [Update the project file](https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-3.1&tabs=visual-studio#update-the-project-file)

A starting list for the package reference is the [WebAPIService.csproj](src/content/src/WebAPIService/WebAPIService.csproj) file.

It is worth clearing the nuget package manager cache and any local package reference folders from the local project.

Then execute a package reinstall command from the Package Manager Console

``` ps
Update-Package -reinstall
```

## Update Startup

The startup class must be modified to align with the default structure for AspNetCore 3.1.

Add a new method ConfigureContainer

``` csharp
    /// <summary>
    /// ConfigureServices is where you register dependencies. This gets
    /// called by the runtime before the ConfigureContainer method, below.
    /// </summary>
    /// <remarks>See https://docs.autofac.org/en/latest/integration/aspnetcore.html#asp-net-core-3-0-and-generic-hosting</remarks>
    /// <param name="builder">The <see cref="ContainerBuilder"/> to configure</param>
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterInstance(_bb).As<IBigBrother>().SingleInstance();
    }
```

Remove code that configures the container from ConfigureServices method

``` csharp
    var builder = new ContainerBuilder();
    builder.Populate(services);
    builder.RegisterInstance(_bb).As<IBigBrother>().SingleInstance();

    // add additional services or modules into container here

    var container = builder.Build();
    return new AutofacServiceProvider(container);
```

Modify the signature for ConfigureServices removing the return of IServiceProvider

``` csharp
public void ConfigureServices(IServiceCollection services)
```

## Update Kestrel

In Program.cs add a using for Microsoft.Extensions.Hosting this enables the use of the new hosting model of AspNetCore.

``` csharp
using Microsoft.Extensions.Hosting;
```

Remove

``` csharp
    var host = WebHost.CreateDefaultBuilder()
        .UseStartup<Startup>()
```

And replace with

``` csharp
    var host = Host
        .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(w=>w.UseStartup<Startup>())
```

The final result is

``` csharp
    else
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(w=>w.UseStartup<Startup>())
            .Build();

        host.Run();
    }
```

## Configure AutoFac

From the changes made so far AutoFac is no longer injected in to AspNetCore. We must now include the relevant code changes to have AutoFac included.

If not already included add a reference to Autofac.Extensions.DependencyInjection

Update the hosting configuration

``` csharp
    else
    {
        var host = Host
            .CreateDefaultBuilder()
            // Add AutoFac support
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(w=>w.UseStartup<Startup>())
            .Build();

        host.Run();
    }
```

See [ASP.NET Core 3.0+ and Generic Hosting](https://docs.autofac.org/en/latest/integration/aspnetcore.html#asp-net-core-3-0-and-generic-hosting)

At this point you should run the the Web Api locally out side of service fabric and test the changes that you have made to this point.

### Update Service Fabric Hosting

Update the service fabric host package reference to the latest version of Microsoft.VisualStudio.Azure.Fabric.MSBuild

``` xml
<package id="Microsoft.VisualStudio.Azure.Fabric.MSBuild" version="1.6.9" targetFramework="net461" />
```

Note that updating the version of the package does not always up date the referenced paths in the service fabric project file.

``` xml
<Import Project="..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props" Condition="Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props')" />
<Import Project="..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props" Condition="Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props')" />
<Import Project="..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets" Condition="Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets')" />
<Import Project="..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets" Condition="Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets')" />
<Error Condition="!Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props')" Text="Unable to find the '..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props' file. Please restore the 'Microsoft.VisualStudio.Azure.Fabric.MSBuild' Nuget package." />
<Error Condition="!Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props')" Text="Unable to find the '..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.props' file. Please restore the 'Microsoft.VisualStudio.Azure.Fabric.MSBuild' Nuget package." />
<Error Condition="!Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets')" Text="Unable to find the '..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets' file. Please restore the 'Microsoft.VisualStudio.Azure.Fabric.MSBuild' Nuget package." />
<Error Condition="!Exists('..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets')" Text="Unable to find the '..\packages\Microsoft.VisualStudio.Azure.Fabric.MSBuild.1.6.9\build\Microsoft.VisualStudio.Azure.Fabric.Application.targets' file. Please restore the 'Microsoft.VisualStudio.Azure.Fabric.MSBuild' Nuget package." />
```

In the CreateServiceInstanceListeners method add the Autofac service to the Host service collection.

``` csharp
    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
    {
        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

        return new WebHostBuilder()
                    .UseKestrel()
                    .ConfigureServices(
                        services => services
                            .AddAutofac() // Add autofac to the host service collection
                            .AddSingleton(serviceContext)
                            .AddSingleton<ITelemetryInitializer>((serviceProvider) => FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(serviceContext))
                            .AddSingleton<ITelemetryModule>(new ServiceRemotingDependencyTrackingTelemetryModule())
                            .AddSingleton<ITelemetryModule>(new ServiceRemotingRequestTrackingTelemetryModule()))
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                    .UseUrls(url)
                    .UseEswSsl(listener)
                    .Build();
    }))
```

**NOTE:** There is an AutoFac extension for adding AutoFac support to Service Fabric see [here](https://docs.autofac.org/en/latest/integration/servicefabric.html) for details. This extension is incompatible with the templates instantiation of the service fabric host.

The service fabric host instantiation creates an instance of a KestrelCommunicationListener which expects an **IWebHostBuilder**, which is supported for backward compatibility see [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.1) for more details.

The host builder created in **Program.cs** is an IHostBuilder derived HostBuilder, this is the recommended host for ASP.Net Core 3.0 and up, see [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1) for more details.
