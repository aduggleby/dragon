using System;
using System.IO;
using Dragon.Files.Interfaces;

namespace Dragon.Files.Storage
{
    public class DragonFileStorage : IFileStorage
    {
        private readonly IFileStorage _fileStorage;

        public DragonFileStorage(IFileStorage storage)
        {
            _fileStorage = storage;
        }

        public IFileStorage StorageImplementation
        {
            get { return _fileStorage; }
        }
        
        /// <summary>
        ///     Uploads a file to either Amazon S3 or the local filesystem as specified in the Dragon configuration file:
        ///     Dragon.Files.Storage = S3 | local
        ///     Further configuration depends on the selected type.
        /// </summary>
        public string Store(string filePath)
        {
            return _fileStorage.Store(filePath);
        }

        public string Store(Stream content, String filePath)
        {
            return _fileStorage.Store(content, filePath);
        }

        /// <summary>
        ///     Retrieves a file from the storage as defined in the configuration.
        ///     See cref="Dragon.Interfaces.Files.IFileStorage.Store(System.String)"/> for details.
        /// </summary>
        public Stream Retrieve(string resourceID)
        {
            return _fileStorage.Retrieve(resourceID);
        }

        public void Delete(string resourceID)
        {
            _fileStorage.Delete(resourceID);
        }

        public bool Exists(string resourceID)
        {
            return _fileStorage.Exists(resourceID);
        }


        public string RetrieveAsUrl(string resourceID)
        {
            return _fileStorage.RetrieveAsUrl(resourceID);
        }
    }
}