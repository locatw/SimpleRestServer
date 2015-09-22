using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestServer
{
    public enum HttpVersion
    {
        Version1_1
    }

    public static class HttpVersionExtension
    {
        public static string ConvertToString(this HttpVersion version)
        {
            switch (version)
            {
                case HttpVersion.Version1_1:
                    return "HTTP/1.1";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
