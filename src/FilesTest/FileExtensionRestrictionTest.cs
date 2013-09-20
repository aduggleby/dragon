using System.Linq;
using Dragon.Interfaces.Files;
using Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesTest
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

        private static IFileRestriction CreateFileRestriction()
        {
            return new FileExtensionRestriction(AllowedExtensions);
        }
    }
}
