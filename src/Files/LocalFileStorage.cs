using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Dragon.Interfaces;
using Dragon.Interfaces.Files;

namespace Files
{
    /// <summary>
    ///     Reads following settings from the configuration:
    ///     Dragon.Files.Local.Path
    /// </summary>
    public class LocalFileStorage : IFileStorage
    {
        private readonly IFileRestriction _fileRestriction;
        private readonly string _path;

        public LocalFileStorage(IConfiguration configuration, IFileRestriction fileRestriction)
        {
            _fileRestriction = fileRestriction;
            _path = configuration.GetValue("Dragon.Files.Local.Path", "");
        }

        public string Store(string filePath)
        {
            if (!File.Exists(filePath)) throw new ResourceToStoreNotFoundException();
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = Guid.NewGuid() + Path.GetExtension(filePath);
            File.Copy(filePath, CreatePath(id));
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

        public ActionResult RetrieveUrl(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
            var mimeMapping = MimeMapping.GetMimeMapping(CreatePath(resourceID));
            // This may lock the resource, if this is an issue clone the stream like in the Retrieve method.
            return new FileStreamResult(new FileStream(CreatePath(resourceID), FileMode.Open), mimeMapping);
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
