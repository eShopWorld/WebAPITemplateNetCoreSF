using System.Threading.Tasks;
using Eshopworld.DevOps;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPIService.Common;
using WebAPIService.Controllers;
using Xunit;

namespace WebAPIService.Tests
{
    public class ProbeTests
    {
        private readonly TestSettings _testSettings;

        public ProbeTests()
        {
            //todo clean this up using the service name rather than LB IP+Port
            _testSettings = new TestSettings();
            var config = EswDevOpsSdk.BuildConfiguration(true);
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
        public async Task Get_DefaultBehaviour_ReturnsContentIntegration()
        {
            var client = new System.Net.Http.HttpClient();

            var result = await client.GetAsync(_testSettings.ProbeEndpoint);

            result.Should().NotBeNull().And.BeOfType<StatusCodeResult>();
            result.StatusCode.Should().Be(200);

            var body = await result.Content.ReadAsStringAsync();
            body.Should().BeNullOrEmpty();
        }
    }
}