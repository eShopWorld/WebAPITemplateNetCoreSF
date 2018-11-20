using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.WebTesting;
using WebAPIService.Common;
using WebAPIService.Performance.Tests.Plugins;

namespace WebAPIService.Performance.Tests
{
    public abstract class CodedWebTestBase : WebTest
    {
        private readonly string _uri;
        private TestSettings _testSettings;
        private readonly ForceTls12Plugin _tlsPlugin = new ForceTls12Plugin();

        /// <summary>
        /// Takes the path from appsettings.json
        /// </summary>
        protected CodedWebTestBase()
        {
            Setup();
            _uri = _testSettings.ApiUri;

        }

        /// <summary>
        /// Takes the base path defined in appsettings.json and combines with supplied path specified
        /// </summary>
        /// <param name="path"></param>
        protected CodedWebTestBase(string path)
        {
            Setup();
            _uri = $"{_testSettings.ApiUri}/{path}";
        }

        /// <summary>
        /// Common setup for the perf tests
        /// </summary>
        private void Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath($"{Path.GetDirectoryName(Environment.CurrentDirectory)}/out")
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");

            var config = builder.Build();

            _testSettings = new TestSettings();
            config.GetSection("TestSettings").Bind(_testSettings);

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
}
