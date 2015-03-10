using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Dragon.Files.Exceptions;
using Dragon.Files.Interfaces;
using Dragon.Files.Storage;

namespace Dragon.Files.MVC
{
    public static class RedirectResultExtensions
    {
        public static ActionResult RetrieveAsActionResult(this DragonFileStorage storage, string resourceID)
        {
            var iFileStorage = storage.StorageImplementation;
            if (iFileStorage is S3FileStorage)
            {
                return ((S3FileStorage) iFileStorage).RetrieveAsActionResult(resourceID);
            }
            else if (iFileStorage is LocalFileStorage)
            {
                return ((LocalFileStorage)iFileStorage).RetrieveAsActionResult(resourceID);
            }

            throw new Exception("Storage type " + storage.GetType().FullName +
                                " does not have an appropriate extension.");
        }


        public static ActionResult RetrieveAsActionResult(this IFileStorage storage, string resourceID)
        {
            var iFileStorage = storage;
            if (iFileStorage is S3FileStorage)
            {
                return ((S3FileStorage)iFileStorage).RetrieveAsActionResult(resourceID);
            }
            else if (iFileStorage is LocalFileStorage)
            {
                return ((LocalFileStorage)iFileStorage).RetrieveAsActionResult(resourceID);
            }

            throw new Exception("Storage type " + storage.GetType().FullName +
                                " does not have an appropriate extension.");
        }

        public static ActionResult RetrieveAsActionResult(this S3FileStorage storage, string resourceID)
        {
            return new RedirectResult(storage.RetrieveAsUrl(resourceID));
        }

        public static ActionResult RetrieveAsActionResult(this LocalFileStorage storage, string resourceID)
        {
            var fs = storage.RetrieveAsFileStream(resourceID);
            var mimeMapping = MimeMapping.GetMimeMapping(resourceID);
            // This may lock the resource, if this is an issue clone the stream like in the Retrieve method.
            return new FileStreamResult(fs, mimeMapping);
        }
    }
}
