using System;

namespace Files
{
    public class FileStoreResourceNotFoundException : Exception
    {
        public FileStoreResourceNotFoundException(string message) : base(message)
        {
        }

        public FileStoreResourceNotFoundException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
