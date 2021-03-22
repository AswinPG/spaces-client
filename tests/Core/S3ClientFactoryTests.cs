using Amazon.S3;
using FluentAssertions;
using Moq;
using SpacesClient.Core.Abstractions;
using Xunit;

namespace SpacesClient.Core.Tests
{
    public class S3ClientFactoryTests
    {
        private readonly S3ClientFactory _factory;

        public S3ClientFactoryTests()
        {
            _factory = new S3ClientFactory();
        }
        
        [Fact]
        public void GivenFactoryWhenCreateS3ClientShouldSetParametersAndReturnInstance()
        {
            const string key = "key";
            const string serviceUrl = "https://nyc3.digitaloceanspaces.com";
            
            var optionsMock = new Mock<ISpacesOptions>();
            optionsMock.SetupGet(x => x.AccessKey)
                .Returns(key)
                .Verifiable();
            optionsMock.SetupGet(x => x.SecretKey)
                .Returns(key)
                .Verifiable();
            optionsMock.SetupGet(x => x.ServiceUrl)
                .Returns(serviceUrl)
                .Verifiable();
            
            var client = _factory.Create(optionsMock.Object);

            client.Should().NotBeNull();
            client.Should().BeOfType<AmazonS3Client>();
            client.Config.ServiceURL.Should().Be(serviceUrl);
            optionsMock.VerifyAll();
        }
    }
}
