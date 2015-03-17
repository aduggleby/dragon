using System;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.EC2;
using Amazon.S3;
using Amazon.S3.Model;
using Dragon.Files.Exceptions;
using Dragon.Files.Interfaces;
using Dragon.Files.S3;

namespace Dragon.Files.Storage
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

        public S3FileStorage(IS3Configuration configuration, IFileRestriction fileRestriction)
        {
            var region = RegionEndpoint.GetBySystemName(configuration.Region);
            
            _fileRestriction = fileRestriction;
            _bucket = configuration.Bucket;
            _client = AWSClientFactory.CreateAmazonS3Client(
                configuration.AccessKeyID,
                configuration.AccessKeySecret,
                region);
        }

        public string Store(string filePath, string contentType)
        {
            if (!File.Exists(filePath)) throw new ResourceToStoreNotFoundException();
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = CreateKey(filePath);
            var request = new PutObjectRequest { BucketName = _bucket, FilePath = filePath, Key = id };
            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
            _client.PutObject(request);
            return id;
        }

        public string Store(Stream content, String filePath, string contentType)
        {
            if (content == null) throw new ResourceToStoreNotFoundException("The passed stream is null.");
            if (!_fileRestriction.IsAllowed(filePath)) throw new FileTypeNotAllowedException();
            var id = CreateKey(filePath);
            var request = new PutObjectRequest { BucketName = _bucket, Key = id, InputStream = content };
            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
            _client.PutObject(request);
            return id;
        }

        public Stream Retrieve(string resourceID)
        {
            var request = new GetObjectRequest { BucketName = _bucket, Key = resourceID };
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


        public string RetrieveAsUrl(string resourceID)
        {
            return GetUrl(resourceID);
        }

        public void Delete(string resourceID)
        {
            if (!Exists(resourceID)) throw new ResourceToRetrieveNotFoundException("Key not found: " + resourceID);
            var deleteObjectRequest = new DeleteObjectRequest { BucketName = _bucket, Key = resourceID };
            _client.DeleteObject(deleteObjectRequest);
        }

        public bool Exists(string resourceID)
        {
            var request = new ListObjectsRequest { BucketName = _bucket };
            var response = _client.ListObjects(request);
            return response.S3Objects.Any(o => o.Key == resourceID);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static string CreateKey(string filePath)
        {
            return Guid.NewGuid() + Path.GetExtension(filePath);
        }

       
        private string GetUrl(string resourceID)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucket,
                Key = resourceID,
                Expires = DateTime.Now.AddHours(1)
            };
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
            return url;
        }
    }
}
