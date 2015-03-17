using System;
using System.IO;
using Dragon.Files.Exceptions;
using Dragon.Files.Interfaces;

namespace Dragon.Files.Storage
{
    /// <summary>
    ///     Reads following settings from the configuration:
    ///     Dragon.Files.Local.Path
    /// </summary>
    public class LocalFileStorage : IFileStorage
    {
        private readonly IFileRestriction _fileRestriction;
        private readonly string _path;

        public LocalFileStorage(ILocalFileConfiguration configuration, IFileRestriction fileRestriction)
        {
            _fileRestriction = fileRestriction;
            _path = configuration.Path;
        }

        public string Store(string filePath, String contentType)
        {
            if (!File.Exists(filePath)) throw new ResourceToStoreNotFoundException();
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = Guid.NewGuid() + Path.GetExtension(filePath);
            File.Copy(filePath, CreatePath(id));
            return id;
        }

        public string Store(Stream content, String filePath, String contentType)
        {
            if (content == null) throw new ResourceToStoreNotFoundException("The passed stream is null.");
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = Guid.NewGuid() + Path.GetExtension(filePath);
            using (var fileStream = File.Create(CreatePath(id)))
            {
                content.CopyTo(fileStream);
            }
            return id;
        }

        public Stream Retrieve(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
            var stream = new MemoryStream();
            using (var fileStream = new FileStream(CreatePath(resourceID), FileMode.Open))
            {
                fileStream.CopyTo(stream);
            }
            stream.Position = 0;
            return stream;
        }

        public FileStream RetrieveAsFileStream(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
            // This may lock the resource, if this is an issue clone the stream like in the Retrieve method.
            return new FileStream(CreatePath(resourceID), FileMode.Open);
        }

        public string RetrieveAsUrl(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
            return Path.GetFullPath(CreatePath(resourceID));
        }

        public void Delete(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
            File.Delete(CreatePath(resourceID));
        }

        public bool Exists(string resourceID)
        {
            return File.Exists(CreatePath(resourceID));
        }

        private string CreatePath(string id)
        {
            return Path.Combine(_path, id);
        }
    }
}
