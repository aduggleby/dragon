using System;
using System.IO;
using Dragon.Interfaces;
using Dragon.Interfaces.Files;

namespace File
{
    /// <summary>
    ///     Reads following settings from the configuration:
    ///     Dragon.Files.Local.Path
    /// </summary>
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _path;

        public LocalFileStorage(IConfiguration configuration)
        {
            _path = configuration.GetValue("Dragon.Files.Local.Path", "");
        }

        public string Store(string filePath)
        {
            var id = Guid.NewGuid().ToString();
            System.IO.File.Copy(filePath, CreatePath(id));
            return id;
        }

        public Stream Retrieve(string resourceID)
        {
            if (!Exists(resourceID)) throw new FileStoreResourceNotFoundException("Key not found: " + resourceID);
            var stream = new MemoryStream();
            using (var fileStream = new FileStream(CreatePath(resourceID), FileMode.Open))
            {
                fileStream.CopyTo(stream);
            }
            stream.Position = 0;
            return stream;
        }

        public void Delete(string resourceID)
        {
            if (!Exists(resourceID)) throw new FileStoreResourceNotFoundException("Key not found: " + resourceID);
            System.IO.File.Delete(CreatePath(resourceID));
        }

        public bool Exists(string resourceID)
        {
            return System.IO.File.Exists(CreatePath(resourceID));
        }

        private string CreatePath(string id)
        {
            return Path.Combine(_path, id + ".dat");
        }
    }
}
