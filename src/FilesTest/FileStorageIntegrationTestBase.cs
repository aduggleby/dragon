using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Dragon.Interfaces.Files;
using Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [ExpectedException(typeof(FileStoreResourceNotFoundException))]
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
        [ExpectedException(typeof(FileStoreResourceNotFoundException))]
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
            Assert.AreEqual("hello s3!\r\n...\r\n..\r\n.\r\n", actual);
        }


        private ControllerContext GetControllerContext()
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.HttpMethod).Returns("GET");
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Request).Returns(request.Object);
            var controllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), new Mock<ControllerBase>().Object);
            return controllerContext;
        }
    }
}
