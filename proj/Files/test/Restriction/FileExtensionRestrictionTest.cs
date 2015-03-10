using System;
using System.Linq;
using Dragon.Files.Interfaces;
using Dragon.Files.Restriction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Files.Test
{
    [TestClass]
    public class FileExtensionRestrictionTest
    {
        private static readonly string[] AllowedExtensions = {"krx"};

        [TestMethod]
        public void IsAllowed_allowedExtension_shouldReturnTrue()
        {
            var fileRestriction = CreateFileRestriction();
            Assert.IsTrue(fileRestriction.IsAllowed("file." + AllowedExtensions.First()));
        }

        [TestMethod]
        public void IsAllowed_disallowedExtension_shouldReturnFalse()
        {
            var fileRestriction = CreateFileRestriction();
            Assert.IsFalse(fileRestriction.IsAllowed("file." + AllowedExtensions.First() + "blub"));            
        }

        [TestMethod]
        public void IsAllowed_noExtension_shouldReturnTrue()
        {
            var fileRestriction = CreateFileRestriction();
            Assert.IsTrue(fileRestriction.IsAllowed("blah"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsAllowed_pathIsNull_shouldThrowNullPointerException()
        {
            var fileRestriction = CreateFileRestriction();
            Assert.IsTrue(fileRestriction.IsAllowed(null));
        }

        private static IFileRestriction CreateFileRestriction()
        {
            return new FileExtensionRestriction(AllowedExtensions);
        }
    }
}
