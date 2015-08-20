using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Xml.XPath;
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
    public class FileFolderTemplateRepositoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ConfigurationMissingException))]
        public void NoBaseDirectoryThrowsException()
        {
            // ARRANGE

            // ACT
            var queue = new FileFolderTemplateRepository(null);

            // ASSERT
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InvalidCultureThrowsException()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_FOLDER))
                .Returns(@"\-hamster-/");
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_DEFLANG))
                .Returns(@"does-not-exist-as-a-culture");

            // ACT
            var queue = new FileFolderTemplateRepository(
                configuration: configMock.Object,
                fileSystem: fsMock.Object);

            // ASSERT
        }

        [TestMethod]
        public void Enumerate_LoadsTextFilesCorrectly()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var fsMock2 = new InMemoryFileSystem(fsMock.Object);

            var defaultCulture = "en-US";
            CreateTemplateSet(fsMock2, @"c:\mock", "t1", null, defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\mock", "t1", "de", defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\mock", "t2", null, defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\notinrightdir", "t3", null, defaultCulture, "txt");

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_FOLDER))
                .Returns(@"c:\mock");
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_DEFLANG))
                .Returns(defaultCulture);

            // ACT
            var queue = new FileFolderTemplateRepository(
                configuration: configMock.Object,
                fileSystem: fsMock2);

            var templates = new List<Template>();
            queue.EnumerateTemplates(templates.Add);

            // ASSERT
            Assert.AreEqual(3, templates.Count);

            AssertTemplateContents(templates, "t1", "en-US");
            AssertTemplateContents(templates, "t1", "de");
            AssertTemplateContents(templates, "t2", "en-US");
        }

        [TestMethod]
        public void Enumerate_LoadsTextAndHtmlFilesCorrectly()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var fsMock2 = new InMemoryFileSystem(fsMock.Object);

            var defaultCulture = "en-US";
            CreateTemplateSet(fsMock2, @"c:\mock", "t1", null, defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\mock", "t1", "de", defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\mock", "t2", null, defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\notinrightdir", "t3", null, defaultCulture, "txt");
            CreateTemplateSet(fsMock2, @"c:\mock", "t1", null, defaultCulture, "html");
            CreateTemplateSet(fsMock2, @"c:\mock", "t1", "de", defaultCulture, "html");
            CreateTemplateSet(fsMock2, @"c:\mock", "t2", null, defaultCulture, "html");
            CreateTemplateSet(fsMock2, @"c:\notinrightdir", "t3", null, defaultCulture, "html");

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_FOLDER))
                .Returns(@"c:\mock");
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_DEFLANG))
                .Returns(defaultCulture);

            // ACT
            var queue = new FileFolderTemplateRepository(
                configuration: configMock.Object,
                fileSystem: fsMock2);

            var templates = new List<Template>();
            queue.EnumerateTemplates(templates.Add);

            // ASSERT
            Assert.AreEqual(3, templates.Count);

            AssertTemplateContents(templates, "t1", "en-US", hasHtml: true);
            AssertTemplateContents(templates, "t1", "de", hasHtml: true);
            AssertTemplateContents(templates, "t2", "en-US", hasHtml: true);
        }

        [TestMethod]
        public void Enumerate_LoadsTextAndHtmlFilesCorrectlyWithincludes()
        {
            // ARRANGE
            var fsMock = new Mock<IFileSystem>();
            fsMock.Setup(m => m.ExistDir(It.IsAny<string>())).Returns(true);

            var fsMock2 = new InMemoryFileSystem(fsMock.Object);
            var basedir = @"c:\mock";
            var defaultCulture = "en-US";

            fsMock2.Save(Path.Combine(basedir, "t1", "body.txt"), "Hi @inc(body2.txt) bob");
            fsMock2.Save(Path.Combine(basedir, "t1", "body2.txt"), "alice");

            fsMock2.Save(Path.Combine(basedir, "t1", "body.html"), "Hi @inc(body2.html) john");
            fsMock2.Save(Path.Combine(basedir, "t1", "body2.html"), "mary");
           
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_FOLDER))
                .Returns(basedir);
            configMock.Setup(m => m.GetValue(FileFolderTemplateRepository.APP_KEY_DEFLANG))
                .Returns(defaultCulture);

            // ACT
            var queue = new FileFolderTemplateRepository(
                configuration: configMock.Object,
                fileSystem: fsMock2);

            var templates = new List<Template>();
            queue.EnumerateTemplates(templates.Add);

            // ASSERT
            Assert.AreEqual(1, templates.Count);

            var t = templates.FirstOrDefault(x => x.Key =="t1" && x.Language.Name.StartsWith("en"));

            Assert.AreEqual("Hi alice bob", t.Content.TextBody);
            Assert.AreEqual("Hi mary john", t.Content.Body);

        }


        private static void AssertTemplateContents( List<Template> templates, string key, string culture, bool hasHtml = false)
        {
            var t = templates.FirstOrDefault(x => x.Key == key && x.Language.Name.StartsWith(culture));
            if (hasHtml)
            {
                Assert.AreEqual(key + culture + "subjectcontents.html", t.Content.Subject);
                Assert.AreEqual(key + culture + "summarysubjectcontents.html", t.Content.SummarySubject);
            }
            else
            {
                Assert.AreEqual(key + culture + "subjectcontents.txt", t.Content.Subject);
                Assert.AreEqual(key + culture + "summarysubjectcontents.txt", t.Content.SummarySubject); 
            }
            Assert.AreEqual(key + culture + "bodycontents.txt", t.Content.TextBody);
            Assert.AreEqual(key + culture + "summarybodycontents.txt", t.Content.SummaryTextBody);
            Assert.AreEqual(key + culture + "summaryfootercontents.txt", t.Content.SummaryTextFooter);
            Assert.AreEqual(key + culture + "summaryheadercontents.txt", t.Content.SummaryTextHeader);
            if (!hasHtml)
            {
                Assert.AreEqual(null, t.Content.Body);
                Assert.AreEqual(null, t.Content.SummaryFooter);
                Assert.AreEqual(null, t.Content.SummaryHeader);
                Assert.AreEqual(null, t.Content.SummaryBody);
            }
            else
            {
                Assert.AreEqual(key + culture + "bodycontents.html", t.Content.Body);
                Assert.AreEqual(key + culture + "summaryfootercontents.html", t.Content.SummaryFooter);
                Assert.AreEqual(key + culture + "summaryheadercontents.html", t.Content.SummaryHeader);
                Assert.AreEqual(key + culture + "summarybodycontents.html", t.Content.SummaryBody);
            }
        }


        private void CreateTemplateSet(IFileSystem fs, string basedir, string key, string culture, string defaultCulture, string ext)
        {
            var types = new string[] { "subject", "body", "summarysubject", "summarybody", "summaryheader", "summaryfooter" };

            foreach (var type in types)
            {
                var filename = Path.Combine(basedir, key, culture ?? String.Empty, type + "." + ext);
                fs.Save(filename, key + (culture ?? defaultCulture) + type + "contents" + "." + ext);
            }

        }

    }
}
