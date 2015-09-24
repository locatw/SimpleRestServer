using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SimpleRestServer
{
    public static class HttpHeaderField
    {
        public enum Connection
        {
            KeepAlive,
            Close
        }
    }

    public static class HttpHeaderFieldExtension
    {
        public static string ConvertToString(this HttpHeaderField.Connection connection)
        {
            switch (connection)
            {
                case HttpHeaderField.Connection.Close:
                    return "close";
                case HttpHeaderField.Connection.KeepAlive:
                    return "keep-alive";
                default:
                    {
                        var message = string.Format("not implemented: {0}", connection.ToString());
                        throw new NotImplementedException(message);
                    }
            }
        }
    }
}
