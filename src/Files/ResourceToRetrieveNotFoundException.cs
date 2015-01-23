using System;

namespace Files
{
    public class ResourceToRetrieveNotFoundException : Exception
    {
        public ResourceToRetrieveNotFoundException(string message) : base(message)
        {
        }

        public ResourceToRetrieveNotFoundException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
