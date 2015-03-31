using System;
using System.IO;
using Dragon.Files.Exceptions;
using Dragon.Files.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Dragon.Files.AzureBlobStorage
{
    public class AzureBlobStorage : IFileStorage
    {
        private readonly IFileRestriction _fileRestriction;
        private readonly CloudBlobContainer _container;

        public AzureBlobStorage(IAzureBlobStorageConfiguration configuration, IFileRestriction fileRestriction)
        {
            _fileRestriction = fileRestriction;
            var storageAccount = CloudStorageAccount.Parse(configuration.StorageConnectionString);
            var client = storageAccount.CreateCloudBlobClient();
            _container = client.GetContainerReference(configuration.Container);
        }

        public string Store(string filePath, string contentType)
        {
            if (!File.Exists(filePath)) throw new ResourceToStoreNotFoundException();
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            string id;
            using (var fileStream = File.OpenRead(filePath))
            {
                id = Store(fileStream, filePath, contentType);
            }
            return id;
        }

        public string Store(Stream content, string filePath, string contentType)
        {
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = CreateKey(filePath);
            var blob = _container.GetBlockBlobReference(id);
            blob.UploadFromStream(content);
            if (!string.IsNullOrEmpty(contentType))
            {
                blob.Properties.ContentType = contentType;
                blob.SetProperties();
            }
            return id;
        }

        public Stream Retrieve(string resourceID)
        {
            var blob = _container.GetBlockBlobReference(resourceID);
            if (!blob.Exists()) {
                throw new ResourceToRetrieveNotFoundException("Unable to retrieve resource.");
            }
            var stream = new MemoryStream();
            blob.DownloadToStream(stream);
            stream.Position = 0;
            return stream;
        }

        public void Delete(string resourceID)
        {
            var blob = _container.GetBlockBlobReference(resourceID);
            if (!blob.Exists())
            {
                throw new ResourceToRetrieveNotFoundException("Unable to retrieve resource.");
            }
            blob.Delete();
        }

        public bool Exists(string resourceID)
        {
            var blob = _container.GetBlockBlobReference(resourceID);
            return blob.Exists();
        }

        public string RetrieveAsUrl(string resourceID)
        {
            return GetUrl(resourceID);
        }

        private static string CreateKey(string filePath)
        {
            return Guid.NewGuid() + Path.GetExtension(filePath);
        }

        private string GetUrl(string resourceID)
        {
            var blob = _container.GetBlockBlobReference(resourceID);
            if (!blob.Exists())
            {
                throw new ResourceToRetrieveNotFoundException("Unable to retrieve resource.");
            }
            var policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                // subtract a few minutes to avoid issues when directly accessing after the URL has been generated
                SharedAccessStartTime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour).Subtract(TimeSpan.FromMinutes(5)),
                SharedAccessExpiryTime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour + 1).AddMinutes(5),
            };

            return blob.Uri + blob.GetSharedAccessSignature(policy);
        }
    }
}
