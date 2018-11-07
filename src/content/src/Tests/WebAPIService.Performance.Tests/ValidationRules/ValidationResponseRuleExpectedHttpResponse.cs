using System.Net;
using Microsoft.VisualStudio.TestTools.WebTesting;

namespace WebAPIService.Performance.Tests.ValidationRules
{
    /// <summary>
    /// Simple validator to check the response of the app and print out the error message it not HTTP 200 in the response
    /// </summary>
    public class ValidationResponseRuleExpectedHttpResponse : ValidationRule
    {
        public override void Validate(object sender, ValidationEventArgs e)
        {
            if (e.Response.StatusCode != HttpStatusCode.OK)
            {
                e.Message = $"Expected HTTP 200 but got HTTP {e.Response.StatusCode} Message {e.Response.BodyString}";
                e.IsValid = false;
            }
        }
    }
}