using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Dragon.Interfaces.Files;
using Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesTest
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
            var fileStorage = new LocalFileStorage(TestHelper.CreateConfigurationMock(new NameValueCollection
            {
                {"Dragon.Files.Local.Path", ConfigurationManager.AppSettings["Dragon.Files.Local.Path"]},
            }).Object);
            return fileStorage;
        }

        [TestMethod]
        public void RetrieveUrl_validFile_shouldReturnUrl()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var stream = (FileStreamResult) fileStorage.RetrieveUrl(id);
            var actual = new StreamReader(stream.FileStream).ReadToEnd();
            stream.FileStream.Close();
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual("hello s3!\r\n...\r\n..\r\n.\r\n", actual);
        }
        
        [TestMethod]
        [ExpectedException(typeof(FileStoreResourceNotFoundException))]
        public void RetrieveUrl_invalidFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.RetrieveUrl(Guid.NewGuid().ToString());
        }
    }
}
