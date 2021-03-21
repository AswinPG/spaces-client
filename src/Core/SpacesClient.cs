using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using SpacesClient.Core.Abstractions;

namespace SpacesClient.Core
{
    public class SpacesClient : ISpacesClient
    {
        private readonly string _bucketName;
        private readonly AmazonS3Config _config;
        private readonly AWSCredentials _credentials;

        public SpacesClient(ISpacesOptions options)
        {
            _credentials = new BasicAWSCredentials(
                options.AccessKey,
                options.SecretKey);
            _config = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl
            };
            _bucketName = options.BucketName;
        }

        public async Task<Stream> DownloadAsync(ISpaceFile file, CancellationToken cancellationToken)
        {
            using var client = new AmazonS3Client(_credentials, _config);
            var response = await client.GetObjectAsync($@"{_bucketName}/{file.CreatedAt:yyyy/MM}", file.Key,
                cancellationToken);
            return response.ResponseStream;
        }

        public async Task DeleteAsync(ISpaceFile file, CancellationToken cancellationToken)
        {
            var bucketName = $@"{_bucketName}/{file.CreatedAt:yyyy/MM}";
            using var client = new AmazonS3Client(_credentials, _config);
            await client.CopyObjectAsync(
                bucketName,
                file.Key,
                $@"{_bucketName}/deleted/{DateTime.UtcNow:yyyy/MM}",
                file.Key,
                cancellationToken);
        }

        public async Task UploadAsync(ICreateSpaceFile createFile, CancellationToken cancellationToken)
        {
            using var client = new AmazonS3Client(_credentials, _config);

            using var transferUtility = new TransferUtility(client);
            var transferRequest = new TransferUtilityUploadRequest
            {
                BucketName = $@"{_bucketName}/{DateTime.UtcNow:yyyy/MM}",
                InputStream = createFile.Stream,
                AutoCloseStream = false,
                ContentType = createFile.ContentType,
                StorageClass = S3StorageClass.Standard,
                PartSize = createFile.Stream.Length,
                Key = createFile.Key,
                CannedACL = S3CannedACL.PublicRead
            };

            await transferUtility.UploadAsync(transferRequest, cancellationToken);
        }
    }
}
