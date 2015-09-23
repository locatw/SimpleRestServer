using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestServer
{
    public class HttpRequestParseException : Exception
    {
        private HttpRequestParseException(HttpRequestParseError error)
            : base()
        {
            Error = error;
        }

        public HttpRequestParseException(HttpRequestParseError error, string message)
            : base(message)
        {
            Error = error;
        }

        public HttpRequestParseException(HttpRequestParseError error, string message, Exception innerException)
            : base(message, innerException)
        {
            Error = error;
        }

        public HttpRequestParseError Error
        {
            get; private set;
        }
    }
}
