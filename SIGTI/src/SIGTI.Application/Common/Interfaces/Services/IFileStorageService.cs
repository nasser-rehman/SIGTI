namespace SIGTI.Application.Common.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(
            Stream stream,
            string storageKey,
            string contentType,
            CancellationToken cancellationToken = default
        );

        Task<Stream> DownloadAsync(
            string storageKey,
            CancellationToken cancellationToken = default
        );

        Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default
        );

        Task<bool> ExistsAsync(
            string storageKey,
            CancellationToken cancellationToken = default
        );
    }
}
