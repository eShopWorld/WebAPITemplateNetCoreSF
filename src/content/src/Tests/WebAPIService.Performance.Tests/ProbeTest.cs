using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using WebAPIService.Common;
using WebAPIService.Performance.Tests.ValidationRules;

namespace WebAPIService.Performance.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [DeploymentItem("tests\\WebAPIService\\appsettings.json")]
    [CodedWebTest]
    [ExcludeFromCodeCoverage]
    public class ProbeTest : CodedWebTestBase
    {
        //todo env may not work depending on where the test is run from ie in vsts, it won't have this env, so it needs to be settings driven
        //todo it should be fine for local dev envs when it's set to development
        /// <summary>
        /// 
        /// </summary>
        public ProbeTest()
        {
            //Directory.GetCurrentDirectory()

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Environment.CurrentDirectory))
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");

            var config = builder.Build();
            var _testSettings = new TestSettings();
            //var eswconfig = EswDevOpsSdk.BuildConfiguration(true);
            config.GetSection("TestSettings").Bind(_testSettings);

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