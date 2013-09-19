using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Dragon.Interfaces.Files;
using File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FileTest
{
    [TestClass]
    public class DragonFileStorageTest
    {
        private const string FILE_TO_STORE_PATH = "packages.config";
        private const string RESOURCE_CONTENT = "ohai!";
        private const string CONFIG_KEY_STORAGE_TYPE = "Dragon.Files.Storage";
        private const string CONFIG_DEFAULT_STORAGE_TYPE = "local";
        private const string CONFIG_VALUE_STORAGE_TYPE_S3 = "S3";

        [TestMethod]
        public void Store_LocalFileStorage_ShouldUseCorrectStorageProvider()
        {
            var expectedResourceIdentifier = Guid.NewGuid().ToString();
            var localFileStorage = CreateFileStorageMock(expectedResourceIdentifier, null);
            var s3FileStorage = CreateFileStorageMock();
            var fileStorage = new DragonFileStorage(
                TestHelper.CreateConfigurationMock(new NameValueCollection
                {
                    {CONFIG_KEY_STORAGE_TYPE, CONFIG_DEFAULT_STORAGE_TYPE}
                }).Object, localFileStorage.Object, s3FileStorage.Object);

            var acutalResourceIdentifier = fileStorage.Store(FILE_TO_STORE_PATH);

            localFileStorage.Verify(x => x.Store(FILE_TO_STORE_PATH), Times.Once());
            s3FileStorage.Verify(x => x.Store(It.IsAny<String>()), Times.Never());

            Assert.AreEqual(expectedResourceIdentifier, acutalResourceIdentifier);
        }

        [TestMethod]
        public void Store_S3FileStorage_ShouldUseCorrectStorageProvider()
        {
            var expectedResourceIdentifier = Guid.NewGuid().ToString();
            var localFileStorage = CreateFileStorageMock();
            var s3FileStorage = CreateFileStorageMock(expectedResourceIdentifier, null);
            var fileStorage = new DragonFileStorage(
                TestHelper.CreateConfigurationMock(new NameValueCollection
                {
                    {CONFIG_KEY_STORAGE_TYPE, CONFIG_VALUE_STORAGE_TYPE_S3}
                }).Object, localFileStorage.Object, s3FileStorage.Object);

            var acutalResourceIdentifier = fileStorage.Store(FILE_TO_STORE_PATH);

            s3FileStorage.Verify(x => x.Store(FILE_TO_STORE_PATH), Times.Once());
            localFileStorage.Verify(x => x.Store(It.IsAny<String>()), Times.Never());

            Assert.AreEqual(expectedResourceIdentifier, acutalResourceIdentifier);
        }

        [TestMethod]
        public void Retrieve_LocalFileStorage_ShouldUseCorrectStorageProvider()
        {
            var resourceIdentifier = Guid.NewGuid().ToString();
            var expectedResourceContent = CreateStreamReader(RESOURCE_CONTENT);
            var localFileStorage = CreateFileStorageMock(resourceIdentifier, CreateStreamReader(RESOURCE_CONTENT));
            var s3FileStorage = CreateFileStorageMock();
            var fileStorage = new DragonFileStorage(
                TestHelper.CreateConfigurationMock(new NameValueCollection
                {
                    {CONFIG_KEY_STORAGE_TYPE, CONFIG_DEFAULT_STORAGE_TYPE}
                }).Object, localFileStorage.Object, s3FileStorage.Object);

            var actualResourceContent = fileStorage.Retrieve(resourceIdentifier);

            localFileStorage.Verify(x => x.Retrieve(resourceIdentifier), Times.Once());
            s3FileStorage.Verify(x => x.Retrieve(It.IsAny<String>()), Times.Never());

            Assert.AreEqual(new StreamReader(expectedResourceContent).ReadToEnd(), new StreamReader(actualResourceContent).ReadToEnd());
            expectedResourceContent.Close();
            actualResourceContent.Close();
        }

        [TestMethod]
        public void Retrieve_S3FileStorage_ShouldUseCorrectStorageProvider()
        {
            var resourceIdentifier = Guid.NewGuid().ToString();
            var expectedResourceContent = CreateStreamReader(RESOURCE_CONTENT);
            var localFileStorage = CreateFileStorageMock();
            var s3FileStorage = CreateFileStorageMock(resourceIdentifier, CreateStreamReader(RESOURCE_CONTENT));
            var fileStorage = new DragonFileStorage(
                TestHelper.CreateConfigurationMock(new NameValueCollection
                {
                    {CONFIG_KEY_STORAGE_TYPE, CONFIG_VALUE_STORAGE_TYPE_S3}
                }).Object, localFileStorage.Object, s3FileStorage.Object);

            var actualResourceContent = fileStorage.Retrieve(resourceIdentifier);

            s3FileStorage.Verify(x => x.Retrieve(resourceIdentifier), Times.Once());
            localFileStorage.Verify(x => x.Retrieve(It.IsAny<String>()), Times.Never());

            Assert.AreEqual(new StreamReader(expectedResourceContent).ReadToEnd(), new StreamReader(actualResourceContent).ReadToEnd());
            expectedResourceContent.Close();
            actualResourceContent.Close();
        }

        # region helper

        private static MemoryStream CreateStreamReader(string ohai)
        {
            return (new MemoryStream(Encoding.Default.GetBytes(ohai)));
        }

        private static Mock<IFileStorage> CreateFileStorageMock()
        {
            return new Mock<IFileStorage>();
        }

        private static Mock<IFileStorage> CreateFileStorageMock(string expectedResourceIdentifier, Stream expectedResourceContent)
        {
            var localFileStorage = new Mock<IFileStorage>();
            localFileStorage.Setup(x => x.Store(It.IsAny<String>())).Returns(expectedResourceIdentifier);
            localFileStorage.Setup(x => x.Retrieve(expectedResourceIdentifier)).Returns(expectedResourceContent);
            return localFileStorage;
        }

        # endregion
    }
}
