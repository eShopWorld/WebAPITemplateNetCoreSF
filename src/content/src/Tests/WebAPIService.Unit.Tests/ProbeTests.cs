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
}
