using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Moq;
using SpacesClient.Core.Abstractions;
using Xunit;

namespace SpacesClient.Core.Tests
{
    public class SpacesClientTests
    {
        private readonly ISpacesClient _client;
        private readonly Mock<IS3ClientFactory> _s3ClientFactoryMock;
        private readonly Mock<ISpacesOptions> _options;
        private readonly Mock<IAmazonS3> _s3Client;

        public SpacesClientTests()
        {
            _s3ClientFactoryMock = new Mock<IS3ClientFactory>(MockBehavior.Strict);
            _options = new Mock<ISpacesOptions>(MockBehavior.Strict);
            _s3Client = new Mock<IAmazonS3>(MockBehavior.Strict);
            _client = new SpacesClient(_s3ClientFactoryMock.Object, _options.Object);
        }

        [Fact]
        public async Task GivenClientWhenDownloadShouldReturnFileStreamFromSpecificBucket()
        {
            const string key = "key";
            const string bucketName = "bucket";
            var createdAt = new DateTime(2021, 1, 1);
            const string bucket = "bucket/2021/01";
            var fileMock = new Mock<ISpaceFile>(MockBehavior.Strict);
            fileMock.SetupGet(x => x.Key)
                .Returns(key)
                .Verifiable();
            fileMock.SetupGet(x => x.CreatedAt)
                .Returns(createdAt)
                .Verifiable();
            _options.SetupGet(x => x.BucketName)
                .Returns(bucketName)
                .Verifiable();
            
            _s3ClientFactoryMock.Setup(x => x.Create(_options.Object))
                .Returns(_s3Client.Object)
                .Verifiable();
            
            _s3Client.Setup(x => x.Dispose())
                .Verifiable();

            _s3Client.Setup(x => x.GetObjectAsync(bucket, key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectResponse()
                {
                    ResponseStream = new MemoryStream()
                })
                .Verifiable();
            
            
            var stream = await _client.DownloadAsync(fileMock.Object, CancellationToken.None);

            stream.Should().NotBeNull();
            fileMock.VerifyAll();
            _options.VerifyAll();
            _s3Client.VerifyAll();
            _s3ClientFactoryMock.VerifyAll();
        }
        
        [Fact]
        public async Task GivenClientWhenUploadShouldSendFileStreamToSpecificBucket()
        {
            const string key = "key";
            const string bucketName = "bucket";
            var createdAt = new DateTime(2021, 1, 1);
            const string bucket = "bucket/2021/01";
            var fileMock = new Mock<ICreateSpaceFile>(MockBehavior.Strict);
            fileMock.SetupGet(x => x.Key)
                .Returns(key)
                .Verifiable();
            fileMock.SetupGet(x => x.ContentType)
                .Returns(MediaTypeNames.Image.Jpeg)
                .Verifiable();
            fileMock.SetupGet(x => x.CreatedAt)
                .Returns(createdAt)
                .Verifiable();
            fileMock.SetupGet(x => x.Stream)
                .Returns(new MemoryStream())
                .Verifiable();
            _options.SetupGet(x => x.BucketName)
                .Returns(bucketName)
                .Verifiable();
            
            _s3ClientFactoryMock.Setup(x => x.Create(_options.Object))
                .Returns(_s3Client.Object)
                .Verifiable();
            
            _s3Client.Setup(x => x.Dispose())
                .Verifiable();

            _s3Client.Setup(x => x.PutObjectAsync(
                    It.Is<PutObjectRequest>(r =>
                            r.Key == key
                            && r.BucketName == bucket
                            && r.ContentType == MediaTypeNames.Image.Jpeg
                            && r.CannedACL == S3CannedACL.PublicRead
                            && r.InputStream is MemoryStream
                            && !r.AutoCloseStream
                            && r.StorageClass == S3StorageClass.Standard),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PutObjectResponse())
                .Verifiable();
            
            
            await _client.UploadAsync(fileMock.Object, CancellationToken.None);

            fileMock.VerifyAll();
            _options.VerifyAll();
            _s3Client.VerifyAll();
            _s3ClientFactoryMock.VerifyAll();
        }
        
        [Fact]
        public async Task GivenClientWhenDeleteShouldMoveFileToDeleteFolder()
        {
            const string key = "key";
            const string bucketName = "bucket";
            var createdAt = new DateTime(2021, 1, 1);
            const string bucket = "bucket/2021/01";
            const string deleteBucket = "bucket/2021/01/deleted";
            var fileMock = new Mock<ISpaceFile>(MockBehavior.Strict);
            fileMock.SetupGet(x => x.Key)
                .Returns(key)
                .Verifiable();
            fileMock.SetupGet(x => x.CreatedAt)
                .Returns(createdAt)
                .Verifiable();
            _options.SetupGet(x => x.BucketName)
                .Returns(bucketName)
                .Verifiable();
            
            _s3ClientFactoryMock.Setup(x => x.Create(_options.Object))
                .Returns(_s3Client.Object)
                .Verifiable();
            
            _s3Client.Setup(x => x.Dispose())
                .Verifiable();

            _s3Client.Setup(x => x.CopyObjectAsync(bucket, key, deleteBucket, key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CopyObjectResponse())
                .Verifiable();
            
            
            await _client.DeleteAsync(fileMock.Object, CancellationToken.None);

            fileMock.VerifyAll();
            _options.VerifyAll();
            _s3Client.VerifyAll();
            _s3ClientFactoryMock.VerifyAll();
        }
    }
}