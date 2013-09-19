using System;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Dragon.Interfaces;
using Dragon.Interfaces.Files;

namespace File
{
    /// <summary>
    ///     Reads following settings from the configuration:
    ///     Dragon.Files.S3.AccessKeyID
    ///     Dragon.Files.S3.AccessKeySecret
    ///     Dragon.Files.S3.Bucke
    /// </summary>
    public class S3FileStorage : IFileStorage, IDisposable
    {
        private readonly string _bucket;
        private readonly IAmazonS3 _client;

        public S3FileStorage(IConfiguration configuration)
        {
            var accessKeyID = configuration.GetValue("Dragon.Files.S3.AccessKeyID", "");
            var accessKeySecret = configuration.GetValue("Dragon.Files.S3.AccessKeySecret", "");
            _bucket = configuration.GetValue("Dragon.Files.S3.Bucket", "");
            _client = AWSClientFactory.CreateAmazonS3Client(
                accessKeyID, accessKeySecret);
        }

        public string Store(string filePath)
        {
            var id = Guid.NewGuid();
            var request = new PutObjectRequest {BucketName = _bucket, FilePath = filePath, Key = id.ToString()};
            var response = _client.PutObject(request);
            string blah;
            response.ResponseMetadata.Metadata.TryGetValue("Key", out blah);
            return id.ToString();
        }

        public Stream Retrieve(string resourceID)
        {
            var request = new GetObjectRequest {BucketName = _bucket, Key = resourceID};
            var memoryStream = new MemoryStream();
            using (var response = _client.GetObject(request))
            {
                response.ResponseStream.CopyTo(memoryStream);
            }
            memoryStream.Seek(0, 0);
            return memoryStream;
        }

        public void Delete(string resourceID)
        {
            if (!Exists(resourceID)) throw new FileStoreResourceNotFoundException("Key not found: " + resourceID);
            var deleteObjectRequest = new DeleteObjectRequest {BucketName = _bucket, Key = resourceID};
            _client.DeleteObject(deleteObjectRequest);
        }

        public bool Exists(string resourceID)
        {
            var request = new ListObjectsRequest {BucketName = _bucket};
            var response = _client.ListObjects(request);
            return response.S3Objects.Any(o => o.Key == resourceID);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
