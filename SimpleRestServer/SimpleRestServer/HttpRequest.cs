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
        private const byte CR = 0x0d;

        private const byte LF = 0x0a;

        public HttpRequest(byte[] requestBytes)
        {
            ParseBytes(requestBytes);
        }

        public HttpRequest(string request)
        {
            Parse(request);
        }

        public HttpRequestMethod Method { get; private set; }

        public HttpVersion Version { get; private set; }

        public string Uri { get; private set; }

        public string UriWithQuery { get; private set; }

        public string Host { get; private set; }

        public IDictionary<string, string> Query { get; private set; }

        private void ParseBytes(byte[] requestBytes)
        {
            MessagePart requestLinePart = FindRequestLinePart(requestBytes);
            MessagePart headerFiledsPart = FindHeaderFieldsPart(requestBytes, requestLinePart);

            string requestLine = MakeMessagePartString(requestBytes, requestLinePart);
            string headerFields = MakeMessagePartString(requestBytes, headerFiledsPart);

            ParseRequestLine(requestLine);
            ParseQueryParameters(UriWithQuery);
            ParseHeaderField(SplitLines(headerFields));
        }

        private MessagePart FindRequestLinePart(byte[] requestBytes)
        {
            int endPosition = Array.FindIndex(requestBytes, b => b == CR);

            return new MessagePart(0, endPosition);
        }

        private MessagePart FindHeaderFieldsPart(byte[] requestBytes, MessagePart requestLinePart)
        {
            int emptyLinePosition = FindEmptyLinePosition(requestBytes, requestLinePart.Length);

            if (emptyLinePosition == -1)
            {
                throw new Exception("invalid HTTP request header format. no empty line exists.");
            }

            int headerFieldsByteCount = emptyLinePosition - (requestLinePart.Length + 2);

            return new MessagePart(requestLinePart.Length + 2, headerFieldsByteCount);
        }

        private int FindEmptyLinePosition(byte[] requestBytes, int requestLineEndPosition)
        {
            int emptyLinePosition = -1;
            for (int i = requestLineEndPosition + 2; i <= requestBytes.Length - 4; i++)
            {
                if (requestBytes[i + 0] == CR &&
                    requestBytes[i + 1] == LF &&
                    requestBytes[i + 2] == CR &&
                    requestBytes[i + 3] == LF)
                {
                    emptyLinePosition = i + 2;
                    break;
                }
            }

            return emptyLinePosition;
        }

        private string MakeMessagePartString(byte[] requestBytes, MessagePart messagePart)
        {
            return Encoding.ASCII.GetString(requestBytes, messagePart.StartIndex, messagePart.Length);
        }

        private void Parse(string request)
        {
            string[] lines = SplitLines(request);

            ParseRequestLine(lines[0]);
            ParseQueryParameters(UriWithQuery);
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

            Uri = elems[1].IndexOf('?') == -1
                        ? elems[1]
                        : elems[1].Substring(0, elems[1].IndexOf('?'));
            UriWithQuery = elems[1];

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

        private string[] SplitLines(string header)
        {
            return header.Split('\n').Select(s => s.Replace("\r", "")).ToArray();
        }

        /// <summary>
        /// This contains start index and byte length for message part
        /// such as request line and header fields in requset bytes.
        /// </summary>
        private class MessagePart
        {
            public MessagePart(int startIndex, int lenght)
            {
                StartIndex = startIndex;
                Length = lenght;
            }

            public int StartIndex { get; private set; }

            public int Length { get; private set; }
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
}
