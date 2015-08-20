using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Test.Mocks;
using Dragon.Mail.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class FileFolderMailQueueTest
    {
        [TestMethod]
        [ExpectedException(typeof(ConfigurationMissingException))]
        public void NoBaseDirectoryThrowsException()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns("");


            // ACT
            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);

            // ASSERT
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InvalidBaseDirectoryThrowsException()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(@"\-hamster-/");


            // ACT
            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);

            // ASSERT
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BaseDirectoryDoesNotExistThrowsException()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(false);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(@"c:\does\not\exist");

            // ACT
            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);

            // ASSERT
        }

        [TestMethod]
        public void BaseDirectoryExistsDoesNotThrowException()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(@"c:\exists");

            // ACT
            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);

            // ASSERT
            fsMock.Verify(m => m.ExistDir(It.Is<string>(s => s == @"c:\exists")), Times.Once);
        }

        [TestMethod]
        public void EnqueueWorks()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);
            fsMock.Setup(m => m.Save(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(true);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(@"c:\maildir");
            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);
            var mail = new Models.Mail();
            mail.Subject = "Subject";

            // ACT
            queue.Enqueue(mail, null);

            // ASSERT
            string expectedFileContents = @"{""$type"":""Dragon.Mail.Models.Mail, Dragon.Mail"",""Subject"":""Subject""}";

            fsMock.Verify(m => m.Save(
                It.IsAny<string>() /* filename */,
                It.Is<string>(s => s == expectedFileContents) /* contents */,
                false /* overwrite */));
        }

        [TestMethod]
        public void EnqueueRetriesFailedSaveAttempts()
        {
            // ARRANGE
            int tries = 0;
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);
            fsMock.Setup(m => m.Save(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(() =>
            {
                if (tries++ < 2) return false;
                return true;
            });

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(@"c:\maildir");
            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);
            var mail = new Models.Mail();
            mail.Subject = "Subject";

            // ACT
            queue.Enqueue(mail, null);

            // ASSERT
            string expectedFileContents = @"{""$type"":""Dragon.Mail.Models.Mail, Dragon.Mail"",""Subject"":""Subject""}";

            fsMock.Verify(m => m.Save(
                It.IsAny<string>() /* filename */,
                It.Is<string>(s => s == expectedFileContents) /* contents */,
                false /* overwrite */), Times.Exactly(3));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EnqueueRetriesFailedSaveAttemptsButFinallyThrowsException()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);
            fsMock.Setup(m => m.Save(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(false);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(@"c:\maildir");

            var queue = new FileFolderMailQueue(fsMock.Object, configMock.Object);

            var mail = new Models.Mail();
            mail.Subject = "Subject";

            // ACT
            queue.Enqueue(mail, null);

            // ASSERT
            const string expectedFileContents = @"{""$type"":""Dragon.Mail.Models.Mail, Dragon.Mail"",""Subject"":""Subject""}";

            fsMock.Verify(m => m.Save(
                It.IsAny<string>() /* filename */,
                It.Is<string>(s => s == expectedFileContents) /* contents */,
                false /* overwrite */), Times.Exactly(3));
        }

        [TestMethod]
        public void DequeueWorks()
        {
            // ARRANGE
            const string DIR = @"c:\maildir";
            var maillist = new Queue<string>();

            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var fsMock2 = new InMemoryFileSystem(fsMock.Object);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderMailQueue.APP_FOLDER))
                .Returns(DIR);

            var queue = new FileFolderMailQueue(fsMock2, configMock.Object);

            // ACT
            // ASSERT

            Assert.IsFalse(queue.Dequeue(m => true));

            queue.Enqueue(new Models.Mail() { Subject = "mail1" }, null);
            queue.Enqueue(new Models.Mail() { Subject = "mail2" }, null);
            queue.Enqueue(new Models.Mail() { Subject = "mail3" }, null);

            Assert.IsTrue(queue.Dequeue(m =>
            {
                Assert.AreEqual("mail1", m.Subject);
                return true;
            }));

            Assert.IsTrue(queue.Dequeue(m =>
            {
                Assert.AreEqual("mail2", m.Subject);
                return true;
            }));

            queue.Enqueue(new Models.Mail() { Subject = "mail4" },null);


            Assert.IsTrue(queue.Dequeue(m =>
            {
                Assert.AreEqual("mail3", m.Subject);
                return true;
            }));


            Assert.IsTrue(queue.Dequeue(m =>
            {
                Assert.AreEqual("mail4", m.Subject);
                return true;
            }));

            Assert.IsFalse(queue.Dequeue(m => true));

        }
    }
}
