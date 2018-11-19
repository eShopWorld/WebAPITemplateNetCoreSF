using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using WebAPIService.Performance.Tests.Plugins;

namespace WebAPIService.Performance.Tests
{
    public abstract class CodedWebTestBase : WebTest
    {
        private readonly string _uri;
        private readonly ForceTls12Plugin _tlsPlugin = new ForceTls12Plugin();

        protected CodedWebTestBase()
        {
            //todo get this from config from env variables
            _uri = "http://localhost:34567/Probe";
            Setup();
        }

        protected CodedWebTestBase(string uri)
        {
            _uri = uri;
            Setup();
        }

        private void Setup()
        {
            _tlsPlugin.Enabled = true;
            PreAuthenticate = true;
            Proxy = "default";
            PreWebTest += _tlsPlugin.PreWebTest;
            PostWebTest += _tlsPlugin.PostWebTest;
            PreTransaction += _tlsPlugin.PreTransaction;
            PostTransaction += _tlsPlugin.PostTransaction;
            PrePage += _tlsPlugin.PrePage;
            PostPage += _tlsPlugin.PostPage;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            yield return new WebTestRequest(_uri) { Encoding = Encoding.GetEncoding("utf-8") };
        }
    }
}
