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
        public class SimpleRequestHeaderParseTest
        {
            private static readonly string requestText =
                "GET / HTTP/1.1" + "\r\n" +
                "Host: sample.jp" + "\r\n" +
                "\r\n";

            [TestClass]
            public class WhenParseFromString
            {
                [TestMethod]
                public void RequestMethodIsGet()
                {
                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual(HttpRequestMethod.Get, request.Method);
                }

                [TestMethod]
                public void VersionIs1_1()
                {
                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual(HttpVersion.Version1_1, request.Version);
                }

                [TestMethod]
                public void RequestUriIsRoot()
                {
                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual("/", request.Uri);
                }

                [TestMethod]
                public void HostFieldIsSampleDotJp()
                {
                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual("sample.jp", request.Host);
                }
            }

            [TestClass]
            public class WhenParseFromBytes
            {
                private byte[] requestBytes = null;

                [TestInitialize]
                public void Initialize()
                {
                    requestBytes = Encoding.ASCII.GetBytes(requestText);
                }
                [TestMethod]
                public void RequestMethodIsGet()
                {
                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual(HttpRequestMethod.Get, request.Method);
                }

                [TestMethod]
                public void VersionIs1_1()
                {
                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual(HttpVersion.Version1_1, request.Version);
                }

                [TestMethod]
                public void RequestUriIsRoot()
                {
                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual("/", request.Uri);
                }

                [TestMethod]
                public void HostFieldIsSampleDotJp()
                {
                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual("sample.jp", request.Host);
                }
            }
        }

        public class RequestWithGetParameterParseTest
        {
            private static readonly string requestTextTemplate =
                "GET /index.html{0} HTTP/1.1" + "\r\n" +
                "Host: sample.jp" + "\r\n" +
                "\r\n";

            [TestClass]
            public class WhenParseFromString
            {
                [TestMethod]
                public void GetValueWithSingleKeyValue()
                {
                    var requestText = String.Format(requestTextTemplate, "?key=value");

                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual("value", request.Query["key"]);
                }

                [TestMethod]
                public void GetValuesWithMultipleKeyValues()
                {
                    var requestText = String.Format(requestTextTemplate, "?key1=value1&key2=value2");

                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual("value1", request.Query["key1"]);
                    Assert.AreEqual("value2", request.Query["key2"]);
                }

                [TestMethod]
                public void UriDoesNotContainsQueryPart()
                {
                    var requestText = String.Format(requestTextTemplate, "?key=value");

                    var request = HttpRequest.Parse(requestText);

                    Assert.AreEqual("/index.html", request.Uri);
                }
            }

            [TestClass]
            public class WhenParseFromBytes
            {
                [TestMethod]
                public void GetValueWithSingleKeyValue()
                {
                    byte[] requestBytes = MakeRequestBytes("?key=value");

                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual("value", request.Query["key"]);
                }

                [TestMethod]
                public void GetValuesWithMultipleKeyValues()
                {
                    byte[] requestBytes = MakeRequestBytes("?key1=value1&key2=value2");

                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual("value1", request.Query["key1"]);
                    Assert.AreEqual("value2", request.Query["key2"]);
                }

                [TestMethod]
                public void UriDoesNotContainsQueryPart()
                {
                    byte[] requestBytes = MakeRequestBytes("?key=value");

                    var request = HttpRequest.Parse(requestBytes);

                    Assert.AreEqual("/index.html", request.Uri);
                }

                private byte[] MakeRequestBytes(string queryPart)
                {
                    var request = String.Format(requestTextTemplate, queryPart);

                    return Encoding.ASCII.GetBytes(request);
                }
            }
        }
    }
}
