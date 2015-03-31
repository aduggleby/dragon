namespace Dragon.Files.AzureBlobStorage
{
    public interface IAzureBlobStorageConfiguration
    {
        string StorageConnectionString { get; }
        string Container { get; }
    }
}