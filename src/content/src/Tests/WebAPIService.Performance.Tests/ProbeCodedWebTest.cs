using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using WebAPIService.Performance.Tests.Plugins;

namespace WebAPIService.Performance.Tests
{
    public class ProbeCodedWebTest : WebTest
    {
        private readonly string _uri;
        private readonly ForceTls12Plugin _tlsPlugin = new ForceTls12Plugin();

        public ProbeCodedWebTest(string serviceName)
        {
            //todo gets this from env config
            _uri = $"https://{serviceName}-we.preprod.eshopworld.net/probe/";

            this.PreAuthenticate = true;
            this.Proxy = "default";
            PreAuthenticate = true;
            _tlsPlugin.Enabled = true;
            Proxy = "default";
            this.PreWebTest += this._tlsPlugin.PreWebTest;
            this.PostWebTest += this._tlsPlugin.PostWebTest;
            this.PreTransaction += this._tlsPlugin.PreTransaction;
            this.PostTransaction += this._tlsPlugin.PostTransaction;
            this.PrePage += this._tlsPlugin.PrePage;
            this.PostPage += this._tlsPlugin.PostPage;
        }

        protected ProbeCodedWebTest(int port)
        {
            //todo get this from env config
            _uri = $"http://40.121.8.42:{port}/Probe";
            //_uri = $"http://40.121.8.42:{port}/Probe";
            PreAuthenticate = true;
            _tlsPlugin.Enabled = true;
            Proxy = "default";
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            yield return new WebTestRequest(_uri) { Encoding = Encoding.GetEncoding("utf-8") };
        }
    }
}
