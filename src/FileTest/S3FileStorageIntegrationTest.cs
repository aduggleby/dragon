using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileTest
{
    /// <summary>
    /// Needs valid Amazon S3 credentials configured in the application configuration file.
    /// See <see cref="S3FileStorage"/> for details.
    /// Note: The tests will upload/download/remove test data to the specified S3 account!
    /// </summary>
    [TestClass]
    public class S3FileStorageIntegrationTest
    {
        private const string TEST_FILE_PATH = "resources/test.txt";

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [ExpectedException(typeof(FileStoreResourceNotFoundException))]
        public void Delete_inexistentFile_shouldThrowException()
        {
            var fileStorage = CreateS3FileStorage();
            fileStorage.Delete("blah");
        }
        
        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Delete_validFile_shouldThrowException()
        {
            var fileStorage = CreateS3FileStorage();
            var id = fileStorage.Store(TEST_FILE_PATH);
            Assert.AreNotEqual("", id);
            fileStorage.Delete(id);
            Assert.AreEqual(false, fileStorage.Exists(id));
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Exists_inexistentFile_shouldReturnFalse()
        {
            var fileStorage = CreateS3FileStorage();
            var actual = fileStorage.Exists(Guid.NewGuid().ToString());
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Exists_validFile_shouldReturnTrue()
        {
            var fileStorage = CreateS3FileStorage();
            var id = fileStorage.Store(TEST_FILE_PATH);
            var actual = fileStorage.Exists(id);
            Assert.AreEqual(true, actual);
            // cleanup
            fileStorage.Delete(id);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Store_validFile_shouldUploadFile()
        {
            var fileStorage = CreateS3FileStorage();
            var id = fileStorage.Store(TEST_FILE_PATH);
            Assert.AreNotEqual("", id);
            Assert.AreEqual(true, fileStorage.Exists(id));
            // cleanup
            fileStorage.Delete(id);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void Retrieve_validID_shouldDownloadFile()
        {
            var fileStorage = CreateS3FileStorage();
            var id = fileStorage.Store(TEST_FILE_PATH);
            var actual = new StreamReader(fileStorage.Retrieve(id)).ReadToEnd();
            Assert.AreEqual("hello s3!\r\n...\r\n..\r\n.\r\n", actual);
            // cleanup
            fileStorage.Delete(id);
        }

        # region helper
        
        private static S3FileStorage CreateS3FileStorage()
        {
            var fileStorage = new S3FileStorage(TestHelper.CreateConfigurationMock(new NameValueCollection
            {
                {"Dragon.Files.S3.AccessKeyID", ConfigurationManager.AppSettings["Dragon.Files.S3.AccessKeyID"]},
                {"Dragon.Files.S3.AccessKeySecret", ConfigurationManager.AppSettings["Dragon.Files.S3.AccessKeySecret"]},
                {"Dragon.Files.S3.Bucket", ConfigurationManager.AppSettings["Dragon.Files.S3.Bucket"]}
            }).Object);
            return fileStorage;
        }

        # endregion helper
    }
}
