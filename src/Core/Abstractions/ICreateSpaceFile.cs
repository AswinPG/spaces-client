using System.IO;

namespace SpacesClient.Core.Abstractions
{
    public interface ICreateSpaceFile : ISpaceFile
    {
        Stream Stream { get; }
        string ContentType { get; }
    }
}