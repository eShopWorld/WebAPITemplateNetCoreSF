using System.Collections.Generic;
using System.IO;
using System.Text;
using Eshopworld.DevOps;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.WebTesting;
using WebAPIService.Performance.Tests.Plugins;

namespace WebAPIService.Performance.Tests
{
    public abstract class CodedWebTestBase : WebTest
    {
        private readonly string _uri;
        private readonly ForceTls12Plugin _tlsPlugin = new ForceTls12Plugin();

        public CodedWebTestBase(string uri)
        {
            _uri = uri;
            _tlsPlugin.Enabled = true;
            PreAuthenticate = true;
            Proxy = "default";
            PreWebTest += _tlsPlugin.PreWebTest;
            PostWebTest += _tlsPlugin.PostWebTest;
            PreTransaction += _tlsPlugin.PreTransaction;
            PostTransaction += _tlsPlugin.PostTransaction;
            PrePage += _tlsPlugin.PrePage;
            PostPage += _tlsPlugin.PostPage;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            yield return new WebTestRequest(_uri) { Encoding = Encoding.GetEncoding("utf-8") };
        }
    }

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
