using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using WebAPIService.Performance.Tests.ValidationRules;

namespace WebAPIService.Performance.Tests
{
    /// <summary>
    /// Perf test on the probe endpoint, making sure that the AspNet Pipeline is working and preformant
    /// </summary>
    [CodedWebTest]
    [ExcludeFromCodeCoverage]
    public class ProbeTest : CodedWebTestBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ProbeTest() : base("Probe")
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
    }
}