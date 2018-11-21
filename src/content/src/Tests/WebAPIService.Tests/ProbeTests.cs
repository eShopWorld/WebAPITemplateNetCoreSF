using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPIService.Common;
using WebAPIService.Controllers;
using Xunit;

namespace WebAPIService.Tests
{
    [ExcludeFromCodeCoverage]
    public class ProbeTests
    {
        private readonly TestSettings _testSettings;

        public ProbeTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Environment.CurrentDirectory))
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true);

            var config = builder.Build();
            _testSettings = new TestSettings();
            config.GetSection("TestSettings").Bind(_testSettings);
        }

        [Fact, IsUnit]
        public void Get_DefaultBehaviour_ReturnsHttp200Unit()
        {
            var controller = new ProbeController();

            var result = controller.Get();

            result.Should().NotBeNull().And.BeOfType<StatusCodeResult>();
            result.StatusCode.Should().Be(200);
        }

        [Fact, IsIntegration]
        public async Task Get_DefaultBehaviour_ReturnsHttp200Integration()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_testSettings.ApiUri)
            };

            var result = await client.GetAsync("Probe");

            result.Should().NotBeNull().And.BeOfType<StatusCodeResult>();
            result.StatusCode.Should().Be(200);

            var body = await result.Content.ReadAsStringAsync();
            body.Should().BeNullOrEmpty();
        }
    }
}