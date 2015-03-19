using System;

namespace Dragon.Security.Hmac.Module.Modules
{
    public class HmacInvalidConfigException : Exception
    {
        public HmacInvalidConfigException(string message) : base(message) { }
    }
}
