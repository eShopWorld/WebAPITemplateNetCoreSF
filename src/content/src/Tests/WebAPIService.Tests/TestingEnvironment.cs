using System.IO;
using Eshopworld.DevOps;
using Microsoft.Extensions.Configuration;


public partial class IntegrationTest
{
    //todo oisin mentioned something like this is in devops, change before merge, if not put this into a core location for use by all
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class TestingEnvironment
    {
        private static bool IsBuildServer => File.Exists(Path.Combine(System.Environment.CurrentDirectory, "appsettings.INTEGRATION.json"));

        private static IConfigurationRoot _configuration;

        public static IConfigurationRoot Configuration
        {
            get
            {
                var currentDir = Directory.GetCurrentDirectory();

                return _configuration ?? (_configuration = EswDevOpsSdk.BuildConfiguration(currentDir, Name, IsBuildServer));
            }
        }

        public static string Name => System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
    }
}
