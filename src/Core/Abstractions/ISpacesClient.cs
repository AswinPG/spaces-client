using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SpacesClient.Core.Abstractions
{
    public interface ISpacesClient
    {
        Task UploadAsync(ICreateSpaceFile file, CancellationToken cancellationToken);
        Task<Stream> DownloadAsync(ISpaceFile file, CancellationToken cancellationToken);
        Task DeleteAsync(ISpaceFile file, CancellationToken cancellationToken);
    }
}