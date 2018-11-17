using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using WebAPIService.Controllers;
using Xunit;

namespace WebAPIService.Tests
{




    public class ProbeTests
    {
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

            var result = await client.GetAsync($"{TestingEnvironment.Configuration["Endpoints:WebAPIService"]}/Probe");

            result.Should().NotBeNull().And.BeOfType<StatusCodeResult>();
            result.StatusCode.Should().Be(200);

            var body = await result.Content.ReadAsStringAsync();
            body.Should().BeNullOrEmpty();
        }
    }
}