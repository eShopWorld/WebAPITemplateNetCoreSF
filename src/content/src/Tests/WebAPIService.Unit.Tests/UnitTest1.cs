using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eshopworld.Tests.Core;
using Xunit;

// ReSharper disable once CheckNamespace
public class UnitTest1
{
    [Fact, IsUnit]
    public async Task Test1()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://security-sts.production.eshopworld.com/.well-known/openid-configuration");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    }
}
