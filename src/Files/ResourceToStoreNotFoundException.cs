using System;

namespace Files
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
