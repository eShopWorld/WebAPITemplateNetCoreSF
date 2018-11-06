using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using WebAPIService.Performance.Tests.Plugins;

namespace WebAPIService.Performance.Tests
{
    public abstract class CodedWebTestBase : WebTest
    {
        private readonly string _uri;
        private readonly ForceTls12Plugin _tlsPlugin = new ForceTls12Plugin();

        /// <summary>
        /// Use when testing the service, region and env only over https
        /// </summary>
        protected CodedWebTestBase(string serviceName, string region, string env)
        {
            _uri = $"https://{serviceName}-{region}.{env}.eshopworld.net/probe/";
            PluginSetup();
        }

        /// <summary>
        /// Use when testing a service and the env over https (via traffic manager)
        /// </summary>
        protected CodedWebTestBase(string serviceName, string env)
        {
            _uri = $"https://{serviceName}.{env}.eshopworld.net/probe/";
            PluginSetup();
        }

        /// <summary>
        /// Testing a service directly over the public load balancer when the service is exposed over http
        /// </summary>
        /// <param name="port"></param>
        protected CodedWebTestBase(int port)
        {
            //todo consider looking this ip up or just use host name instead.
            //don't care about change too much now.
            _uri = $"http://40.121.8.42:{port}/Probe";
            _tlsPlugin.Enabled = true;
            //_uri = $"http://40.121.8.42:{port}/Probe";
            PreAuthenticate = true;
            Proxy = "default";
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            //get the env here

            yield return new WebTestRequest(_uri) { Encoding = Encoding.GetEncoding("utf-8") };
        }

        /// <summary>
        /// Shared plumbing for plugin setup
        /// </summary>
        private void PluginSetup()
        {
            _tlsPlugin.Enabled = true;
            this.PreAuthenticate = true;
            this.Proxy = "default";
            this.PreWebTest += _tlsPlugin.PreWebTest;
            this.PostWebTest += _tlsPlugin.PostWebTest;
            this.PreTransaction += _tlsPlugin.PreTransaction;
            this.PostTransaction += _tlsPlugin.PostTransaction;
            this.PrePage += _tlsPlugin.PrePage;
            this.PostPage += _tlsPlugin.PostPage;
        }
    }

    public class ProbeTest : CodedWebTestBase
    {
        /// <summary>
        /// Use when testing a region and the service is exposed over https
        /// </summary>
        /// <param name="region"></param>
        /// <param name="env"></param>
        public ProbeTest(string region, string env) : base("WebAPIService", region, env)
        {
        }

        //todo env may not work depending on where the test is run from ie in vsts, it won't have this env, so it needs to be settings driven
        //todo it should be fine for local dev envs when it's set to development
        public ProbeTest() : base("WebAPIService", EnvHelper.GetCurrentEnvironment)
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

    /// <summary>
    /// todo move to esw.web
    /// </summary>
    public class EnvHelper
    {
        public static string GetCurrentEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

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
