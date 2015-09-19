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

        public string Host { get; private set; }

        public IDictionary<string, string> Query { get; private set; }

        private void Parse(string request)
        {
            string[] lines = request.Split('\n')
                                .Select(s => s.Replace("\r", ""))
                                .ToArray();

            ParseRequestLine(lines[0]);
            ParseQueryParameters(Uri);
            ParseHeaderField(lines.Skip(1).ToArray());
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

        private void ParseQueryParameters(string uri)
        {
            if (!uri.Contains("?"))
            {
                return;
            }

            string query = uri.Substring(uri.IndexOf('?') + 1);
            string[] keyValues = query.Split('&');

            Query =
                keyValues
                    .Select(keyValue => keyValue.Split('='))
                    .ToDictionary(keyValue => keyValue[0], keyValue => keyValue[1]);

        }

        private void ParseHeaderField(string[] headerLines)
        {
            var fieldLines =
                headerLines
                    .Where(line => line.Contains(':'))
                    .Select(line => HeaderFieldLine.Create(line));

            foreach (var field in fieldLines)
            {
                if (field.Field == "Host")
                {
                    Host = field.Values;
                }
            }
        }

        private class HeaderFieldLine
        {
            private HeaderFieldLine()
            { }

            public static HeaderFieldLine Create(string fieldLine)
            {
                string[] parts = fieldLine.Split(':');

                var headerFieldLine = new HeaderFieldLine();
                headerFieldLine.Field = parts[0];
                headerFieldLine.Values = parts[1].TrimStart();

                return headerFieldLine;
            }

            public string Field { get; private set; }

            public string Values { get; private set; }
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
}
