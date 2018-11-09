using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using WebAPIService.Controllers;
using Xunit;

// ReSharper disable once CheckNamespace
public partial class ProbeTests
{
    [Fact, IsUnit]
    public void Get_DefaultBehaviour_ReturnsHttp200()
    {
        var controller = new ProbeController();

        var result = controller.Get();

        result.Should().NotBeNull().And.BeOfType<OkResult>();
        result.Should().BeNull();
    }

    [Fact, IsIntegration]
    public async Task Get_DefaultBehaviour_ReturnsContent()
    {
        var client = new System.Net.Http.HttpClient();

        var result = await client.GetAsync($"{TestingEnvironment.Configuration["Endpoints:WebAPIService"]}/Probe");

        result.Should().NotBeNull();
        result.IsSuccessStatusCode.Should().BeTrue();

        var body = await result.Content.ReadAsStringAsync();
        body.Should().BeNullOrEmpty();
    }
}
