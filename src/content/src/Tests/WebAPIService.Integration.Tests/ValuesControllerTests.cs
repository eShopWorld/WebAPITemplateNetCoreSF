using System;
using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Xunit;

// ReSharper disable once CheckNamespace
public class ValuesControllerTests
{
    private Xunit.Abstractions.ITestOutputHelper Console;

    public ValuesControllerTests(Xunit.Abstractions.ITestOutputHelper console)
    {
        Console = console;
    }

    [Fact, IsIntegration]
    public async Task Get_DefaultBehaviour_ReturnsContent()
    {
        // Arrange
        var client = new System.Net.Http.HttpClient();

        // Act
        var result = await client.GetAsync(new Uri(WebAPIService.Integration.Tests.Environment.APIEndpoint,"/api/v1/Values"));

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessStatusCode.Should().BeTrue();

        var body = await result.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrEmpty();

        Console.WriteLine(body);
    }
}
