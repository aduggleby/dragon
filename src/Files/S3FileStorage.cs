using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Dragon.Interfaces;
using Dragon.Interfaces.Files;

namespace Files
{
    /// <summary>
    ///     Reads following settings from the configuration:
    ///     Dragon.Files.S3.AccessKeyID
    ///     Dragon.Files.S3.AccessKeySecret
    ///     Dragon.Files.S3.Bucket
    /// </summary>
    public class S3FileStorage : IFileStorage, IDisposable
    {
        private readonly IFileRestriction _fileRestriction;
        private readonly string _bucket;
        private readonly IAmazonS3 _client;

        public S3FileStorage(IConfiguration configuration, IFileRestriction fileRestriction)
        {
            _fileRestriction = fileRestriction;
            var accessKeyID = configuration.GetValue("Dragon.Files.S3.AccessKeyID", "");
            var accessKeySecret = configuration.GetValue("Dragon.Files.S3.AccessKeySecret", "");
            _bucket = configuration.GetValue("Dragon.Files.S3.Bucket", "");
            _client = AWSClientFactory.CreateAmazonS3Client(accessKeyID, accessKeySecret);
        }

        public string Store(string filePath)
        {
            if (!File.Exists(filePath)) throw new ResourceToStoreNotFoundException();
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = Guid.NewGuid() + Path.GetExtension(filePath);
            var request = new PutObjectRequest {BucketName = _bucket, FilePath = filePath, Key = id};
            _client.PutObject(request);
            return id;
        }

        public Stream Retrieve(string resourceID)
        {
            var request = new GetObjectRequest {BucketName = _bucket, Key = resourceID};
            var memoryStream = new MemoryStream();
            try
            {
                using (var response = _client.GetObject(request))
                {
                    response.ResponseStream.CopyTo(memoryStream);
                }
            }
            catch (AmazonS3Exception e)
            {
                // Catching exception instead of checking if the resource exists beforehand for performance reasons only.
                throw new ResourceToRetrieveNotFoundException("Unable to retrieve resource.", e);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        public ActionResult RetrieveUrl(string resourceID)
        {
            var request = new GetPreSignedUrlRequest {BucketName = _bucket, Key = resourceID, Expires = DateTime.Now.AddHours(1)};
            string url;
            try
            {
                 url = _client.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception e)
            {
                // Catching exception instead of checking if the resource exists beforehand for performance reasons only.
                throw new ResourceToRetrieveNotFoundException("Unable to retrieve resource.", e);
            }
            return new RedirectResult(FixUrl(url));
        }

        private static string FixUrl(string url)
        {
            var subDomain = new Uri(url).PathAndQuery.Split('/')[1];
            return url.Replace(subDomain + "/", "").Replace("//", "//" + subDomain + ".");
        }

        public void Delete(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
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
