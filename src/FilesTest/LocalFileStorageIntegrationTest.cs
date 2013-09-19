using System.Collections.Specialized;
using System.Configuration;
using Dragon.Interfaces.Files;
using Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesTest
{
    /// <summary>
    /// Needs an existing path configured in the application configuration file.
    /// See <see cref="LocalFileStorage"/> for details.
    /// </summary>
    [TestClass]
    public class LocalFileStorageIntegrationTest : FileStorageIntegrationTestBase
    {
        public override IFileStorage CreateFileStorage()
        {
            var fileStorage = new LocalFileStorage(TestHelper.CreateConfigurationMock(new NameValueCollection
            {
                {"Dragon.Files.Local.Path", ConfigurationManager.AppSettings["Dragon.Files.Local.Path"]},
            }).Object);
            return fileStorage;
        }
    }
}
