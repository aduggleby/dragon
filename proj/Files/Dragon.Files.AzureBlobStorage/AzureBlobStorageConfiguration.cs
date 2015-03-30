using System.Configuration;

namespace Dragon.Files.AzureBlobStorage
{
    public class AzureBlobStorageConfiguration : IAzureBlobStorageConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string Container { get; set; }

        public static AzureBlobStorageConfiguration FromAppConfig()
        {
            return new AzureBlobStorageConfiguration()
            {
                StorageConnectionString = ConfigurationManager.AppSettings["Dragon.Files.AzureBlobStorage.StorageConnectionString"],
                Container = ConfigurationManager.AppSettings["Dragon.Files.AzureBlobStorage.Container"],
            };
        }
    }
}
