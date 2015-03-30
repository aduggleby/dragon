using Dragon.Files.AzureBlobStorage;
using Dragon.Files.Interfaces;
using Dragon.Files.Restriction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Files.Test
{
    /// <summary>
    /// Needs valid Azure Blob Storage credentials configured in the application configuration file.
    /// See <see cref="AzureBlobFileStorage"/> for details.
    /// </summary>
    [TestClass]
    public class AzureBlobStorageFileStorageIntegrationTest : S3FileStorageIntegrationTest
    {
        public override IFileStorage CreateFileStorage()
        {
            var fileStorage = new AzureBlobStorage.AzureBlobStorage(AzureBlobStorageConfiguration.FromAppConfig(), 
                new FileExtensionRestriction(FileExtensionRestriction.GetDefaultAllowedFileTypes()));
            return fileStorage;
        }
    }
}
