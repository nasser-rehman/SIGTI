using Microsoft.Extensions.Options;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Infrastructure.Storage
{
    public sealed class LocalStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public LocalStorageService(IOptions<FileStorageOptions> options)
        {
            var configuredPath = options?.Value?.BasePath;
            _basePath = Path.GetFullPath(
                string.IsNullOrWhiteSpace(configuredPath)
                    ? Path.Combine(
                        AppContext.BaseDirectory,
                        "App_Data",
                        "Uploads"
                    )
                    : configuredPath
            );
        }

        public async Task<string> UploadAsync(
            Stream stream,
            string storageKey,
            string contentType,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException(
                    "A chave de armazenamento é obrigatória.",
                    nameof(storageKey)
                );

            var fullPath = GetSafeFullPath(storageKey);
            var directory = Path.GetDirectoryName(fullPath);

            if (
                !string.IsNullOrEmpty(directory) && !Directory.Exists(directory)
            )
            {
                Directory.CreateDirectory(directory);
            }

            if (stream.CanSeek)
                stream.Position = 0;

            await using var fileStream = new FileStream(
                fullPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true
            );

            await stream.CopyToAsync(fileStream, cancellationToken);

            return storageKey;
        }

        public Task<Stream> DownloadAsync(
            string storageKey,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException(
                    "A chave de armazenamento é obrigatória.",
                    nameof(storageKey)
                );

            var fullPath = GetSafeFullPath(storageKey);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException(
                    $"Arquivo de chave '{storageKey}' não encontrado."
                );

            Stream stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true
            );

            return Task.FromResult(stream);
        }

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(storageKey))
                return Task.CompletedTask;

            var fullPath = GetSafeFullPath(storageKey);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(
            string storageKey,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(storageKey))
                return Task.FromResult(false);

            var fullPath = GetSafeFullPath(storageKey);
            return Task.FromResult(File.Exists(fullPath));
        }

        private string GetSafeFullPath(string storageKey)
        {
            var sanitizedKey = storageKey.Replace('\\', '/').TrimStart('/');
            var combinedPath = Path.Combine(_basePath, sanitizedKey);
            var fullPath = Path.GetFullPath(combinedPath);

            // Path Traversal mitigation
            if (
                !fullPath.StartsWith(
                    _basePath,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new InvalidOperationException(
                    "Tentativa de acesso a caminho inválido fora do diretório de armazenamento."
                );
            }

            return fullPath;
        }
    }
}
