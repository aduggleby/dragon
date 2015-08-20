using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class InMemoryFileSystemTest
    {
        [TestMethod]
        public void Basic()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";

            var file1a = Path.Combine(dir1, @"file1");

            // ACT
            subject.Save(file1a, file1a);

            // ASSERT
            var actual= subject.PeekOldest(dir1);
            Assert.AreEqual(file1a, actual);

        }

        [TestMethod]
        public void RemoveWorks()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";

            var file1a = Path.Combine(dir1, @"file1");

            // ACT
            subject.Save(file1a, file1a);
            subject.Remove(file1a);

            // ASSERT
            var actual = subject.PeekOldest(dir1);
            Assert.IsNull(actual);
        }


        [TestMethod]
        public void MoveWorks()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";
            var dir2 = @"c:\temp\two";

            var file1a = Path.Combine(dir1, @"file1");

            // ACT
            subject.Save(file1a, file1a);
            subject.Move(file1a, dir2);

            // ASSERT
            var actual = subject.PeekOldest(dir1);
            Assert.IsNull(actual);

            Assert.AreEqual(file1a, subject.PeekOldest(dir2));

        }
        [TestMethod]
        public void MultipleFiles()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";

            var file1a = Path.Combine(dir1, @"file1");
            var file1b = Path.Combine(dir1, @"file2");

            // ACT
            subject.Save(file1a, file1a);
            subject.Save(file1b, file1b);

            // ASSERT
            var actual1 = subject.PeekOldest(dir1);
            Assert.AreEqual(file1a, actual1);
            subject.Remove(file1a);

            var actual2 = subject.PeekOldest(dir1);
            Assert.AreEqual(file1b, actual2);
        }

        [TestMethod]
        public void MultipleFilesMultipleDirectories()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";
            var dir2 = @"c:\temp\two";

            var file1a = Path.Combine(dir1, @"file1");
            var file1b = Path.Combine(dir1, @"file2");

            var file2a = Path.Combine(dir2, @"file1");
            var file2b = Path.Combine(dir2, @"file2");

            // ACT
            subject.Save(file1a, file1a);
            subject.Save(file2a, file2a);
            subject.Save(file1b, file1b);
            subject.Save(file2b, file2b);

            // ASSERT
            Assert.AreEqual(file1a, subject.PeekOldest(dir1));
            subject.Remove(file1a);
            Assert.AreEqual(file1b, subject.PeekOldest(dir1));
            Assert.AreEqual(file2a, subject.PeekOldest(dir2));
            subject.Remove(file1b);
            Assert.AreEqual(file2a, subject.PeekOldest(dir2));
            subject.Remove(file2a);
            Assert.AreEqual(file2b, subject.PeekOldest(dir2));

        }


        [TestMethod]
        public void EnumerateGetAllFilesInDir()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";
            var dir11 = @"c:\temp\one\a";
            var dir12 = @"c:\temp\one\b";

            var dir2 = @"c:\temp\two";

            var file1a = Path.Combine(dir1, @"file1");
            var file1b = Path.Combine(dir1, @"file2");

            var file11a = Path.Combine(dir11, @"file11");
            var file12a = Path.Combine(dir12, @"file12");
            
            var file2a = Path.Combine(dir2, @"file1");
            var file2b = Path.Combine(dir2, @"file2");

            subject.Save(file1a, file1a);
            subject.Save(file2a, file2a);
            subject.Save(file1b, file1b);
            subject.Save(file2b, file2b);
            subject.Save(file11a, file11a);
            subject.Save(file12a, file12a);

            var subdirs0 = new List<string>();
            var subdirs1 = new List<string>();
            var subdirs2 = new List<string>();
            
            // ACT
            subject.EnumerateDirectory(@"c:\temp", subdirs0.Add);
            subject.EnumerateDirectory(dir1, subdirs1.Add);
            subject.EnumerateDirectory(dir2, subdirs2.Add);

            // ASSERT
            Assert.AreEqual(2, subdirs0.Count);
            Assert.AreEqual("one", subdirs0.First());
            Assert.AreEqual("two", subdirs0.Last());

            Assert.AreEqual(2, subdirs1.Count);
            Assert.AreEqual("a", subdirs1.First());
            Assert.AreEqual("b", subdirs1.Last());

            Assert.AreEqual(0, subdirs2.Count);            
        }

        [TestMethod]
        public void GetContentsReadFiles()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var subject = new InMemoryFileSystem(fsMock.Object);

            var dir1 = @"c:\temp\one";
            var dir2 = @"c:\temp\two";

            var file1a = Path.Combine(dir1, @"file1");
            var file1b = Path.Combine(dir1, @"file2");

            var file2a = Path.Combine(dir2, @"file1");
            var file2b = Path.Combine(dir2, @"file2");

            subject.Save(file1a, file1a);
            subject.Save(file2a, file2a);
            subject.Save(file1b, file1b);
            subject.Save(file2b, file2b);

          
            // ACT
            var contents = subject.GetContents(file1a.ToUpper());

            // ASSERT
            Assert.AreEqual(file1a, contents);
            
        }

    }
}
