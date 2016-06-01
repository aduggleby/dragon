using System;

namespace Dragon.SecurityServer.AccountSTS.WebRequestHandler
{
    public class OpenIdMigrationException : Exception
    {
        public OpenIdMigrationException(string message) : base(message)
        {
        }
    }
}