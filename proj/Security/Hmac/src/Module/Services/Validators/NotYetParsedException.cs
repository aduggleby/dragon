using System;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class NotYetParsedException : Exception
    {
        public NotYetParsedException(string message) : base(message)
        {
        }
    }
}
