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

            [TestMethod]
            public void HostFieldIsSampleDotJp()
            {
                var request = new HttpRequest(requestText);

                Assert.AreEqual("sample.jp", request.Host);
            }
        }

        [TestClass]
        public class RequestWithGetParameterParseTest
        {
            private readonly string requestTextTemplate =
                "GET /index.html{0} HTTP/1.1" + "\r\n" +
                "Host: sample.jp" + "\r\n";

            [TestMethod]
            public void GetValueWithSingleKeyValue()
            {
                var requestText = String.Format(requestTextTemplate, "?key=value");

                var request = new HttpRequest(requestText);

                Assert.AreEqual("value", request.Query["key"]);
            }

            [TestMethod]
            public void GetValuesWithMultipleKeyValues()
            {
                var requestText = String.Format(requestTextTemplate, "?key1=value1&key2=value2");

                var request = new HttpRequest(requestText);

                Assert.AreEqual("value1", request.Query["key1"]);
                Assert.AreEqual("value2", request.Query["key2"]);
            }

            [TestMethod]
            public void UriDoesNotContainsQueryPart()
            {
                var requestText = String.Format(requestTextTemplate, "?key=value");

                var request = new HttpRequest(requestText);

                Assert.AreEqual("/index.html", request.Uri);
            }
        }
    }
}
