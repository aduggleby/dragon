using System;

namespace File
{
    public class FileStoreResourceNotFoundException : Exception
    {
        public FileStoreResourceNotFoundException(string message) : base(message)
        {
        }
    }
}
