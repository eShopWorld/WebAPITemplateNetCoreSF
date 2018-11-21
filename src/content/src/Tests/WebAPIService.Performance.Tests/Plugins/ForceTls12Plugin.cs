using System.ComponentModel;
using System.Net;
using Microsoft.VisualStudio.TestTools.WebTesting;

namespace WebAPIService.Performance.Tests.Plugins
{
    //todo move to core location if does not exist already and expose
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
}
