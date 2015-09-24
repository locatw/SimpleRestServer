using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestServer
{
    public class HttpResponse
    {
        public HttpResponse(HttpVersion version, HttpStatus status)
        {
            Version = version;
            Status = status;
            Content = String.Empty;
            Connection = HttpHeaderField.Connection.KeepAlive;
        }

        public byte[] ToByteArray()
        {
            var builder = new StringBuilder();

            builder.Append(Version.ConvertToString());
            builder.Append(" ");

            builder.Append(Status.ToString());
            builder.Append("\r\n");

            if (HasContent)
            {
                builder.Append(String.Format("Content-Length: {0}", Encoding.ASCII.GetByteCount(Content)));
                builder.Append("\r\n");
            }

            builder.AppendFormat("Connection: {0}", Connection.ConvertToString());
            builder.Append("\r\n");

            builder.Append("\r\n");

            byte[] headerBytes = Encoding.ASCII.GetBytes(builder.ToString());

            if (HasContent)
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(Content);

                return headerBytes.Concat(bodyBytes).ToArray();
            }
            else
            {
                return headerBytes;
            }
        }

        public void SetContent(string content)
        {
            Content = content;
        }

        public void SetConnection(HttpHeaderField.Connection connection)
        {
            Connection = connection;
        }

        public HttpVersion Version { get; private set; }

        public HttpStatus Status { get; private set; }

        public HttpHeaderField.Connection Connection { get; private set; }

        public string Content { get; private set; }

        public bool HasContent
        {
            get
            {
                return Content != String.Empty;
            }
        }
    }
}
