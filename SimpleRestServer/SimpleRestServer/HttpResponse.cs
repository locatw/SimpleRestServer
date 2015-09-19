using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestServer
{
    public class HttpResponse
    {
        public HttpResponse(HttpStatus status)
        {
            Status = status;
        }

        public HttpStatus Status { get; private set; }
    }
}
