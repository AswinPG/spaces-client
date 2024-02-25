using SpacesClient.Core.Abstractions;

namespace SpacesClient.Core.Implementations
{
    public class SpaceOptions : ISpacesOptions
    {
        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string ServiceUrl { get; set; }

        public string BucketName { get; set; }

        public SpaceOptions(string accesskey, string secretkey, string serviceurl, string bucketname)
        {
            AccessKey = accesskey;
            SecretKey = secretkey;
            ServiceUrl = serviceurl;
            BucketName = bucketname;
        }
    }
}
