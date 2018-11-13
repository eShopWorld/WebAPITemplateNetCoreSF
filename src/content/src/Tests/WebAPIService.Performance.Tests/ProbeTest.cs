using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using WebAPIService.Performance.Tests.ValidationRules;

namespace WebAPIService.Performance.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [CodedWebTest]
    public class ProbeTest : CodedWebTestBase
    {
        //todo env may not work depending on where the test is run from ie in vsts, it won't have this env, so it needs to be settings driven
        //todo it should be fine for local dev envs when it's set to development
        /// <summary>
        /// 
        /// </summary>
        public ProbeTest() : base(Uri)
        {
            if (this.Context.ValidationLevel >= ValidationLevel.High)
            {
                var expectedHttpValidationRule = new ValidationResponseRuleExpectedHttpResponse();
                this.ValidateResponse += expectedHttpValidationRule.Validate;

                var expectedHttpResponseTime = new ValidationRuleResponseTimeGoal
                {
                    Tolerance = 10D
                };
                this.ValidateResponse += expectedHttpResponseTime.Validate;
            }
        }

        public static string Uri => $"{TestingEnvironment.Configuration["Endpoints:WebAPIService"]}/Probe";
    }
}