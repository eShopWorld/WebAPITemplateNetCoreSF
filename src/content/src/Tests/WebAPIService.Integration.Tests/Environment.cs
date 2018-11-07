using System;
using Eshopworld.DevOps;
using Microsoft.Extensions.Configuration;

namespace WebAPIService.Integration.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Environment
    {
        static Environment()
        {
            var isBuildServer = System.IO.File.Exists(System.IO.Path.Combine(System.Environment.CurrentDirectory, "appsettings.INTEGRATION.json"));
            Name = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;

            Configuration = EswDevOpsSdk.BuildConfiguration(System.Environment.CurrentDirectory, Name, isBuildServer);
            APIEndpoint = new Uri(Configuration["Endpoints:WebAPIServiceApi"]);
        }

        public static IConfigurationRoot Configuration { get; }
        public static string Name { get; }
        public static Uri APIEndpoint { get; }
    }
}