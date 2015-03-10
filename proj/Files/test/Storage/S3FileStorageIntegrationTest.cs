using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using Dragon.Files.Exceptions;
using Dragon.Files.Interfaces;
using Dragon.Files.MVC;
using Dragon.Files.Restriction;
using Dragon.Files.S3;
using Dragon.Files.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Files.Test
{
    /// <summary>
    /// Needs valid Amazon S3 credentials configured in the application configuration file.
    /// See <see cref="S3FileStorage"/> for details.
    /// </summary>
    [TestClass]
    public class S3FileStorageIntegrationTest : FileStorageIntegrationTestBase
    {
        public override IFileStorage CreateFileStorage()
        {
            var fileStorage = new S3FileStorage(S3Configuration.FromAppConfig(), 
                new FileExtensionRestriction(FileExtensionRestriction.GetDefaultAllowedFileTypes()));
            return fileStorage;
        }

        [Ignore] // S3 does not throw an exception in this case...
        [TestMethod]
        [ExpectedException(typeof(ResourceToRetrieveNotFoundException))]
        public void RetrieveAsActionResult_invalidFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.RetrieveAsActionResult(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void RetrieveAsActionResult_validFile_shouldReturnActionResult()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var actualUrl = ((RedirectResult)fileStorage.RetrieveAsActionResult(id)).Url;
            var actual = new WebClient().DownloadString(actualUrl);
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual(TestFileContent, actual);
        }

        [Ignore] // S3 does not throw an exception in this case...
        [TestMethod]
        [ExpectedException(typeof(ResourceToRetrieveNotFoundException))]
        public void RetrieveAsUrl_invalidFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.RetrieveAsUrl(Guid.NewGuid().ToString());
        }


 
        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void RetrieveAsUrl_validFile_shouldRetrieveUrl()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var actual = new WebClient().DownloadString(fileStorage.RetrieveAsUrl(id));
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual(TestFileContent, actual);
        }
    }
}
