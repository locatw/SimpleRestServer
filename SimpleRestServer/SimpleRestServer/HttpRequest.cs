using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace SimpleRestServer
{
    public class HttpRequest
    {
        public HttpRequest(string request)
        {
            Parse(request);
        }

        public HttpRequestMethod Method { get; private set; }

        public HttpVersion Version { get; private set; }

        public string Uri { get; private set; }

        private void Parse(string request)
        {
            string[] lines = request.Split('\n')
                                .Select(s => s.Replace("\r", ""))
                                .ToArray();

            ParseRequestLine(lines[0]);
        }

        private void ParseRequestLine(string line)
        {
            string[] elems = line.Split(' ');

            HttpRequestMethod method;
            if (Enum.TryParse(elems[0], true, out method))
            {
                this.Method = method;
            }
            else
            {
                throw new Exception("unknown request method.");
            }

            this.Uri = elems[1];

            switch (elems[2])
            {
                case "HTTP/1.1":
                    this.Version = HttpVersion.Version1_1;
                    break;
                default:
                    throw new Exception("unsupported http version");
            }
        }
    }

    public enum HttpRequestMethod
    {
        Get
    }

    public enum HttpVersion
    {
        Version1_1
    }

    public enum HttpRequestHeaderField
    {
        Host
    }
}
