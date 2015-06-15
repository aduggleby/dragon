using System;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class DependencyMissingException : Exception
    {
        public DependencyMissingException(string message) : base(message)
        {
        }

        public DependencyMissingException()
        {
        }
    }
}
