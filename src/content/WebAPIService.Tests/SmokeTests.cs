using Eshopworld.DevOps;
using Eshopworld.Tests.Core;
using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebAPIService.Tests
{
    public class SmokeTests
    {
        [Fact, IsLayer2]
        public async Task ProbePing()
        {
            var config = EswDevOpsSdk.BuildConfiguration();
            var url = config["ProbeUrl"];

            HttpClient cl = new HttpClient();
            var resp = await cl.GetAsync(url);
            resp.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
