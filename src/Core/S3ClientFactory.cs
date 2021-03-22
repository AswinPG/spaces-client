using Amazon.Runtime;
using Amazon.S3;
using SpacesClient.Core.Abstractions;

namespace SpacesClient.Core
{
    public interface IS3ClientFactory
    {
        IAmazonS3 Create(ISpacesOptions options);
    }
    
    public class S3ClientFactory : IS3ClientFactory
    {
        public IAmazonS3 Create(ISpacesOptions options)
        {
            var credentials = new BasicAWSCredentials(
                options.AccessKey,
                options.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl
            };
            return new AmazonS3Client(credentials, config);
        }
    }
}