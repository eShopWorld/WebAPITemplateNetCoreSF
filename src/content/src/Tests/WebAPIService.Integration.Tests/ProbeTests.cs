using System;
using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Xunit;

// ReSharper disable once CheckNamespace
public class IntegrationTests
{
    [Fact, IsIntegration]
    public async Task Get_DefaultBehaviour_ReturnsContent()
    {
        var client = new System.Net.Http.HttpClient();

        var result = await client.GetAsync($"{IntegrationTest.TestingEnvironment.Configuration["Endpoints:WebAPIService"]}/Probe");

        result.Should().NotBeNull();
        result.IsSuccessStatusCode.Should().BeTrue();

        var body = await result.Content.ReadAsStringAsync();
        body.Should().BeNullOrEmpty();
    }
}
