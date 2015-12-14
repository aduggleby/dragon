using System;

namespace Dragon.SecurityServer.GenericSTSClient
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }
    }
}
