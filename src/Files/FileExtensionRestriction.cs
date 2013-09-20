using System.IO;
using System.Linq;
using Dragon.Interfaces.Files;

namespace Files
{
    public class FileExtensionRestriction : IFileRestriction
    {
        private readonly string[] _allowedFileExtensions;

        public FileExtensionRestriction(string[] allowedFileExtensions)
        {
            _allowedFileExtensions = allowedFileExtensions;
        }

        public bool IsAllowed(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return extension != null && _allowedFileExtensions.Contains(extension.TrimStart('.'));
        }

        public static string[] GetDefaultAllowedFileTypes()
        {
            return new[]
            {
                "doc", "docx", "odt", // writer
                "xls", "xlsx", "ods", // spreadsheet
                "ppt", "pptx", "odp", // presentation
                "txt", "pdf", "ps", "epub", "tex", // documents
                "css", "xml", "html", "xslt",
                "zip", "rar", "7zip", "tar", "gz", // packer
                "bmp", "jpg", "jpeg", "tif", "png", "gif", // raster graphics
                "svg", // vector graphics
                "wav", "mp3", // audio
                "mpeg", "avi" // video
            };
        }
    }
}
