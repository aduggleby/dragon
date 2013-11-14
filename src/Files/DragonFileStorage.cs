using System;
using System.IO;
using System.Web.Mvc;
using Dragon.Interfaces;
using Dragon.Interfaces.Files;

namespace Files
{
    public class DragonFileStorage : IFileStorage
    {
        private const string CONFIG_KEY_STORAGE_TYPE = "Dragon.Files.Storage";
        private const string CONFIG_DEFAULT_STORAGE_TYPE = "local";
        private const string CONFIG_VALUE_STORAGE_TYPE_S3 = "S3";

        private readonly IFileStorage _fileStorage;

        public DragonFileStorage(IConfiguration configuration, IFileStorage localFileStorage, IFileStorage s3FileStorage)
        {
            _fileStorage = configuration.GetValue(
                CONFIG_KEY_STORAGE_TYPE, CONFIG_DEFAULT_STORAGE_TYPE) == CONFIG_VALUE_STORAGE_TYPE_S3
                ? s3FileStorage
                : localFileStorage;
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

        public ActionResult RetrieveUrl(string resourceID)
        {
            return _fileStorage.RetrieveUrl(resourceID);
        }
    }
}