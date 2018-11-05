using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

namespace WebAPIService.Performance.Tests
{
    /// <summary>
    /// This class will override the Security Protocol being used by the Performance Web Test to TLS 1.2
    /// </summary>
    /// <seealso cref="WebTestPlugin" />
    [DisplayName("Change TLS to 1.2.")]
    [Description("Changes the security protocol to TLS 1.2.")]
    public class ForceTls12Plugin : WebTestPlugin
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ForceTls12Plugin"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [Description("Enable or Disable the plugin functionality")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// It will override the Security Protocol being used by the Performance Web Test to TLS 1.2..
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="T:Microsoft.VisualStudio.TestTools.WebTesting.PostWebTestEventArgs" /> that contains the event data.</param>
        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            base.PreWebTest(sender, e);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            e.WebTest.AddCommentToResult(ToString() +
                                         " has made the following modification-> ServicePointManager.SecurityProtocol set to use Tls12 in WebTest Plugin.");
        }
    }

    public class WebTest1Coded : WebTest
    {

        public WebTest1Coded()
        {
            this.PreAuthenticate = true;
            this.Proxy = "default";
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            ForLoopRule conditionalRule1 = new ForLoopRule();
            conditionalRule1.ContextParameterName = "i";
            conditionalRule1.ComparisonOperator = ForLoopComparisonOperator.LessThan;
            conditionalRule1.TerminatingValue = 100D;
            conditionalRule1.InitialValue = 0D;
            conditionalRule1.IncrementValue = 1D;

            int maxIterations1 = 100;
            bool advanceDataCursors1 = false;
            this.BeginLoop(conditionalRule1, maxIterations1, advanceDataCursors1);

            for (; this.ExecuteConditionalRule(conditionalRule1);)
            {
                yield break;
            }

            this.EndLoop(conditionalRule1);

            LastResponseCodeRule conditionalRule2 = new LastResponseCodeRule();
            conditionalRule2.ComparisonOperator = StringComparisonOperator.Equality;
            conditionalRule2.ResponseCode = WebTestResponseCode.Ok;

            this.BeginCondition(conditionalRule2);

            if (this.ExecuteConditionalRule(conditionalRule2))
            {
                yield break;
            }

            this.EndCondition(conditionalRule2);

            //todo need a "directory" way of looking this up for any env or if we're always sticking with convention then it's pretty simple but need to know http/https route
            WebTestRequest request1 = new WebTestRequest("http://localhost:WebAPIServicePort/Probe");
            request1.Encoding = System.Text.Encoding.GetEncoding("utf-8");
            yield return request1;
            request1 = null;
        }
    }
}
