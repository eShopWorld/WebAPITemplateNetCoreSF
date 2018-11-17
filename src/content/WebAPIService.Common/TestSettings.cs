namespace WebAPIService.Common
{
    public class TestSettings
    {
        public string ApiUri { get; set; }

        public string ProbeEndpoint => $"{ApiUri}/Probe";
    }
}