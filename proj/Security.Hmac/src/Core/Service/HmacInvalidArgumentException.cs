using System;

namespace Dragon.Security.Hmac.Core.Service
{
    public class HmacInvalidArgumentException : Exception
    {
        public HmacInvalidArgumentException(string message) : base(message) { }
    }
}
