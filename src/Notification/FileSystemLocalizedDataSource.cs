using System;
using System.IO;
using Dragon.Interfaces.Notifications;

namespace Dragon.Notification
{
    public class FileSystemLocalizedDataSource : ILocalizedDataSource
    {
        private readonly string _baseDirectory;
        private readonly string _fileExtension;

        public FileSystemLocalizedDataSource(String baseDirectory, String fileExtension)
        {
            _baseDirectory = baseDirectory;
            _fileExtension = fileExtension;
        }

        public string GetContent(string key, string languageCode)
        {
            return File.ReadAllText(Path.Combine(
                _baseDirectory, string.Format("{0}_{1}.{2}", key, languageCode, _fileExtension)));
        }
    }
}
