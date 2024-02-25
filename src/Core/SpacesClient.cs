using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SpacesClient.Core.Abstractions;

namespace SpacesClient.Core
{
    public class SpacesClient : ISpacesClient
    {
        private readonly IS3ClientFactory _factory;
        private readonly ISpacesOptions _options;

        public SpacesClient(IS3ClientFactory factory, ISpacesOptions options)
        {
            _factory = factory;
            _options = options;
        }

        public async Task<Stream> DownloadAsync(ISpaceFile file, CancellationToken cancellationToken)
        {
            using var client = _factory.Create(_options);
            var response = await client.GetObjectAsync($@"{_options.BucketName}/{file.CreatedAt:yyyy/MM}", file.Key,
                cancellationToken);
            return response.ResponseStream;
        }

        public async Task DeleteAsync(ISpaceFile file, CancellationToken cancellationToken)
        {
            using var client = _factory.Create(_options);
            var bucketName = $@"{_options.BucketName}/{file.CreatedAt:yyyy/MM}";
            await client.CopyObjectAsync(
                bucketName,
                file.Key,
                $@"{_options.BucketName}/{file.CreatedAt:yyyy/MM}/deleted",
                file.Key,
                cancellationToken);
        }

        public async Task UploadAsync(ICreateSpaceFile createFile, CancellationToken cancellationToken)
        {
            using var client = _factory.Create(_options);
            using var transferUtility = new TransferUtility(client);
            var transferRequest = new TransferUtilityUploadRequest
            {
                BucketName = $@"{_options.BucketName}/{createFile.CreatedAt:yyyy/MM}",
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

        public async Task<string> UploadAsync(ICreateSpaceFile createFile, CancellationToken cancellationToken, Action<int> progressAction)
        {
            try
            {
                using IAmazonS3 client = _factory.Create(_options);
                using TransferUtility transferUtility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
                {
                    BucketName = (_options.BucketName ?? ""),
                    InputStream = createFile.Stream,
                    AutoCloseStream = false,
                    ContentType = createFile.ContentType,
                    StorageClass = S3StorageClass.Standard,
                    PartSize = createFile.Stream.Length,
                    Key = createFile.Key,
                    CannedACL = S3CannedACL.PublicRead
                };

                request.UploadProgressEvent += ProgressEvent;

                await transferUtility.UploadAsync(request, cancellationToken);
                request.UploadProgressEvent -= ProgressEvent;

                void ProgressEvent(object sender, UploadProgressArgs e)
                {
                    Console.WriteLine($"Uploaded {e.TransferredBytes} bytes out of {e.TotalBytes}");
                    progressAction?.Invoke(e.PercentDone);
                }
                return "https://" + _options.BucketName + ".sgp1.cdn.digitaloceanspaces.com/" + createFile.Key;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        public async Task<string> DeleteFromPathAsync(string url)
        {
            string key = url.Split(_options.BucketName + ".sgp1.cdn.digitaloceanspaces.com/")[1];
            return await DeleteAsync(_options.BucketName, key);
        }
        async Task<string> DeleteAsync(string bucket, string key)
        {
            using IAmazonS3 client = _factory.Create(_options);
            return (await client.DeleteObjectAsync(bucket, key)).DeleteMarker;
        }
    }
}
