using System;

namespace SimpleRestServer
{
    public class HttpStatus
    {
        public static readonly HttpStatus Ok =
            new HttpStatus(200, "OK");

        public static readonly HttpStatus BadRequest =
            new HttpStatus(400, "Bad Request");

        public static readonly HttpStatus NotFound =
            new HttpStatus(404, "Not Found");

        public static readonly HttpStatus InternalServerError =
            new HttpStatus(500, "Internal Server Error");

        public static readonly HttpStatus HttpVersionNotSupported =
            new HttpStatus(505, "HTTP Version Not Supported");

        public HttpStatus(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpStatus);
        }

        public bool Equals(HttpStatus obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Code == obj.Code && Description.Equals(obj.Description);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Description.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Code, Description);
        }

        public static bool operator ==(HttpStatus obj1, HttpStatus obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(HttpStatus obj1, HttpStatus obj2)
        {
            return !obj1.Equals(obj2);
        }

        public int Code { get; private set; }

        public string Description { get; private set; }
    }
}
