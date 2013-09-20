using System;
using System.IO;
using Dragon.Interfaces.Files;
using Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesTest
{
    /// <summary>
    /// Needs valid configuration provided in the application configuration file.
    /// See concrete test classes for details.
    /// Note: The tests will upload/download/remove test data to the storage provider!
    /// </summary>
    [TestClass]
    public abstract class FileStorageIntegrationTestBase
    {
        public abstract IFileStorage CreateFileStorage();

        protected const string TestFilePath = "resources/test.txt";
        protected const string DisallowedTestFilePath = "resources/test.php";
        protected const string TestFileContent = "hello s3!\r\n...\r\n..\r\n.\r\n";

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [ExpectedException(typeof(ResourceToRetrieveNotFoundException))]
        public void Delete_inexistentFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.Delete(Guid.NewGuid().ToString());
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Delete_validFile_shouldDeleteFile()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            Assert.AreNotEqual("", id);
            fileStorage.Delete(id);
            Assert.AreEqual(false, fileStorage.Exists(id));
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Exists_inexistentFile_shouldReturnFalse()
        {
            var fileStorage = CreateFileStorage();
            var actual = fileStorage.Exists(Guid.NewGuid().ToString());
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Exists_validFile_shouldReturnTrue()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var actual = fileStorage.Exists(id);
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Store_validFile_shouldUploadFile()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var exists = fileStorage.Exists(id);
            fileStorage.Delete(id); // cleanup
            Assert.AreNotEqual("", id);
            Assert.AreEqual(true, exists);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [ExpectedException(typeof(ResourceToStoreNotFoundException))]
        public void Store_invalidFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.Store(TestFilePath + "doesnotexist");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [ExpectedException(typeof(FileTypeNotAllowedException))]
        public void Store_disallowedFile_shouldThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.Store(DisallowedTestFilePath);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [ExpectedException(typeof(ResourceToRetrieveNotFoundException))]
        public void Retrieve_invalidFile_shoulThrowException()
        {
            var fileStorage = CreateFileStorage();
            fileStorage.Retrieve(Guid.NewGuid().ToString());
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Retrieve_validFile_shouldDownloadFile()
        {
            var fileStorage = CreateFileStorage();
            var id = fileStorage.Store(TestFilePath);
            var actual = new StreamReader(fileStorage.Retrieve(id)).ReadToEnd();
            fileStorage.Delete(id); // cleanup
            Assert.AreEqual(TestFileContent, actual);
        }

    }
}
