using System;

namespace Dragon.Files.Exceptions
{
    public class ResourceToStoreNotFoundException : Exception
    {
        public ResourceToStoreNotFoundException()
        {
        }

        public ResourceToStoreNotFoundException(string message) : base(message)
        {
        }
    }
}
