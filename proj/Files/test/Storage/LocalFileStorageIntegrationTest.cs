using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Dragon.Files.Exceptions;
using Dragon.Files.Interfaces;
using Dragon.Files.Restriction;
using Dragon.Files.Storage;
using Dragon.Files.MVC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Files.Test
{
    /// <summary>
    /// Needs an existing path configured in the application configuration file.
    /// See <see cref="LocalFileStorage"/> for details.
    /// </summary>
    [TestClass]
    public class LocalFileStorageIntegrationTest : FileStorageIntegrationTestBase
    {
        public override IFileStorage CreateFileStorage()
        {
            var dir = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            var fileStorage = new LocalFileStorage(new LocalFileConfiguration()
            {
                Path = dir
            },
            new FileExtensionRestriction(FileExtensionRestriction.GetDefaultAllowedFileTypes()));
            return fileStorage;
        }

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
            var stream = (FileStreamResult)fileStorage.RetrieveAsActionResult(id);
            var actual = new StreamReader(stream.FileStream).ReadToEnd();
            stream.FileStream.Close();
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual(TestFileContent, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceToRetrieveNotFoundException))]
        public void RetrieveAsUrl_invalidFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.RetrieveAsUrl(Guid.NewGuid().ToString());
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void RetrieveAsUrl_validFile_shouldReturnUrl()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var actual = File.ReadAllText(fileStorage.RetrieveAsUrl(id));
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual(TestFileContent, actual);
        }
    }
}
