namespace SpacesClient.Core.Abstractions
{
    public interface ISpacesOptions
    {
        string AccessKey { get; }
        string SecretKey { get; }
        string ServiceUrl { get; }
        string BucketName { get; }
    }
}