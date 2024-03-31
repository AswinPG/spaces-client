namespace SpacesClient.Core.Abstractions
{
    public interface ISpacesOptions
    {
        string AccessKey { get; }
        string SecretKey { get; }
        string ServiceUrl { get; }
        string CdnUrl { get; }
        string BucketName { get; }
    }
}