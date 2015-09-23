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
        private HttpRequest()
        { }

        public static HttpRequest Parse(byte[] requestBytes)
        {
            return new HttpRequestParser().Parse(requestBytes);
        }

        public static HttpRequest Parse(string requestString)
        {
            return new HttpRequestParser().Parse(requestString);
        }

        public HttpRequestMethod Method { get; private set; }

        public HttpVersion Version { get; private set; }

        public string Uri { get; private set; }

        public string UriWithQuery { get; private set; }

        public string Host { get; private set; }

        public IDictionary<string, string> Query { get; private set; }

        private class HttpRequestParser
        {
            private const byte CR = 0x0d;

            private const byte LF = 0x0a;

            public HttpRequest Parse(byte[] requestBytes)
            {
                try
                {
                    MessagePart requestLinePart = FindRequestLinePart(requestBytes);
                    MessagePart headerFiledsPart = FindHeaderFieldsPart(requestBytes, requestLinePart);

                    string requestLineString = MakeMessagePartString(requestBytes, requestLinePart);
                    string headerFieldsString = MakeMessagePartString(requestBytes, headerFiledsPart);

                    var constructingRequest = new HttpRequest();

                    ParseRequestLine(requestLineString, constructingRequest);
                    ParseQueryParameters(constructingRequest);
                    ParseHeaderField(SplitLines(headerFieldsString), constructingRequest);

                    return constructingRequest;
                }
                catch (HttpRequestParseException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new HttpRequestParseException(
                        HttpRequestParseError.UnknownError, "unknown error occurred", e);
                }
            }

            public HttpRequest Parse(string request)
            {
                try
                {
                    string[] lines = SplitLines(request);
                    var constructingRequest = new HttpRequest();

                    ParseRequestLine(lines[0], constructingRequest);
                    ParseQueryParameters(constructingRequest);
                    ParseHeaderField(lines.Skip(1).ToArray(), constructingRequest);

                    return constructingRequest;
                }
                catch (HttpRequestParseException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new HttpRequestParseException(
                        HttpRequestParseError.UnknownError, "unknown error occurred", e);
                }
            }

            private MessagePart FindRequestLinePart(byte[] requestBytes)
            {
                int endPosition = Array.FindIndex(requestBytes, b => b == CR);

                if (endPosition != -1)
                {
                    return new MessagePart(0, endPosition);
                }
                else
                {
                    throw new HttpRequestParseException(
                        HttpRequestParseError.InvalidFormat, "new line does not exists");
                }
            }

            private MessagePart FindHeaderFieldsPart(byte[] requestBytes, MessagePart requestLinePart)
            {
                int emptyLinePosition = FindEmptyLinePosition(requestBytes, requestLinePart.Length);

                if (emptyLinePosition != -1)
                {
                    int headerFieldsByteCount = emptyLinePosition - (requestLinePart.Length + 2);

                    return new MessagePart(requestLinePart.Length + 2, headerFieldsByteCount);
                }
                else
                {
                    throw new HttpRequestParseException(
                            HttpRequestParseError.InvalidFormat, "empty line does not exists");
                }
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

            private void ParseRequestLine(string line, HttpRequest constructingRequest)
            {
                string[] elems = line.Split(' ');
                if (elems.Length != 3)
                {
                    throw new HttpRequestParseException(
                        HttpRequestParseError.InvalidFormat, "invalid request line format");
                }

                HttpRequestMethod method;
                if (Enum.TryParse(elems[0], true, out method))
                {
                    constructingRequest.Method = method;
                }
                else
                {
                    var message = String.Format("unknown request method: {0}", elems[0]);

                    throw new HttpRequestParseException(
                        HttpRequestParseError.UnknownMethod, message);
                }

                constructingRequest.Uri =
                    elems[1].IndexOf('?') == -1
                        ? elems[1]
                        : elems[1].Substring(0, elems[1].IndexOf('?'));
                constructingRequest.UriWithQuery = elems[1];

                switch (elems[2])
                {
                    case "HTTP/1.1":
                        constructingRequest.Version = HttpVersion.Version1_1;
                        break;
                    default:
                        {
                            var message = String.Format("unsupported HTTP version: {0}", elems[2]);

                            throw new HttpRequestParseException(
                                HttpRequestParseError.UnsupportedVersion, message);
                        }
                }
            }

            private void ParseQueryParameters(HttpRequest constructingRequest)
            {
                string uriWithQuery = constructingRequest.UriWithQuery;

                if (!uriWithQuery.Contains("?"))
                {
                    return;
                }

                string query = uriWithQuery.Substring(uriWithQuery.IndexOf('?') + 1);
                string[] keyValues = query.Split('&');

                constructingRequest.Query =
                    keyValues
                        .Select(keyValue => keyValue.Split('='))
                        .ToDictionary(keyValue => keyValue[0], keyValue => keyValue[1]);

            }

            private void ParseHeaderField(string[] headerLines, HttpRequest constructingRequest)
            {
                var fieldLines =
                    headerLines
                        .Where(line => line.Contains(':'))
                        .Select(line => HeaderFieldLine.Create(line));

                foreach (var field in fieldLines)
                {
                    if (field.Field == "Host")
                    {
                        constructingRequest.Host = field.Values;
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
}
