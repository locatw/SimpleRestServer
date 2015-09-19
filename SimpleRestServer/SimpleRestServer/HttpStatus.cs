using System;

namespace SimpleRestServer
{
    public class HttpStatus
    {
        public static readonly HttpStatus Ok = new HttpStatus(200, "OK");

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
