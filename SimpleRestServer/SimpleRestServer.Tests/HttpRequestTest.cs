using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestServer.Tests
{
    public class HttpRequestTest
    {
        [TestClass]
        public class SimpleRequestHeaderParseTest
        {
            private readonly string requestText =
                "GET / HTTP/1.1" + "\r\n" +
                "Host: sample.jp" + "\r\n";

            [TestMethod]
            public void RequestMethodIsGet()
            {
                var request = new HttpRequest(requestText);

                Assert.AreEqual(HttpRequestMethod.Get, request.Method);
            }

            [TestMethod]
            public void VersionIs1_1()
            {
                var request = new HttpRequest(requestText);

                Assert.AreEqual(HttpVersion.Version1_1, request.Version);
            }

            [TestMethod]
            public void RequestUriIsRoot()
            {
                var request = new HttpRequest(requestText);

                Assert.AreEqual("/", request.Uri);
            }
        }
    }
}
