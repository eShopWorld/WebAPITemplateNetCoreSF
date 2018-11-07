using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using WebAPIService.Controllers;
using Xunit;

// ReSharper disable once CheckNamespace
public class ValuesControllerTests
{
    [Fact, IsUnit]
    public async Task Get_DefaultBehaviour_ReturnsJsonResult()
    {
        // Arrange
        var controller = new ValuesController();

        // Act
        var result = await controller.Get();

        // Assert
        result.Should().NotBeNull().And.BeOfType<JsonResult>();
    }
}
