using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Services
{
    public sealed class FileSecurityValidator : IFileSecurityValidator
    {
        private static readonly Dictionary<
            string,
            List<byte[]>
        > FileSignatures = new(StringComparer.OrdinalIgnoreCase)
        {
            [".png"] =
            [
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
            ],
            [".jpeg"] =
            [
                [0xFF, 0xD8, 0xFF],
            ],
            [".jpg"] =
            [
                [0xFF, 0xD8, 0xFF],
            ],
            [".pdf"] =
            [
                [0x25, 0x50, 0x44, 0x46], // %PDF
            ],
            [".zip"] =
            [
                [0x50, 0x4B, 0x03, 0x04], // PK..
                [0x50, 0x4B, 0x05, 0x06], // PK.. (empty)
                [0x50, 0x4B, 0x07, 0x08], // PK.. (spanned)
            ],
        };

        private static readonly HashSet<string> AllowedExtensions = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".pdf",
            ".zip",
            ".txt",
            ".log",
        };

        private static readonly HashSet<string> AllowedMimeTypes = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            "image/png",
            "image/jpeg",
            "application/pdf",
            "application/zip",
            "application/x-zip-compressed",
            "text/plain",
        };

        public bool IsAllowedExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                && AllowedExtensions.Contains(extension);
        }

        public bool IsAllowedContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                return false;

            return AllowedMimeTypes.Contains(contentType.Trim());
        }

        public async Task<bool> ValidateFileSignatureAsync(
            Stream stream,
            string fileName,
            CancellationToken cancellationToken = default
        )
        {
            if (stream is null || !stream.CanRead)
                return false;

            var extension = Path.GetExtension(fileName);
            if (
                string.IsNullOrEmpty(extension)
                || !AllowedExtensions.Contains(extension)
            )
                return false;

            if (
                extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".log", StringComparison.OrdinalIgnoreCase)
            )
            {
                return await ValidatePlainTextAsync(stream, cancellationToken);
            }

            if (!FileSignatures.TryGetValue(extension, out var signatures))
                return false;

            var maxSignatureLength = signatures.Max(s => s.Length);
            var headerBytes = new byte[maxSignatureLength];

            var originalPosition = stream.CanSeek ? stream.Position : 0;
            if (stream.CanSeek)
                stream.Position = 0;

            var bytesRead = await stream.ReadAsync(
                headerBytes.AsMemory(0, maxSignatureLength),
                cancellationToken
            );

            if (stream.CanSeek)
                stream.Position = originalPosition;

            if (bytesRead == 0)
                return false;

            return signatures.Any(sig =>
                bytesRead >= sig.Length
                && headerBytes.Take(sig.Length).SequenceEqual(sig)
            );
        }

        private static async Task<bool> ValidatePlainTextAsync(
            Stream stream,
            CancellationToken cancellationToken
        )
        {
            var buffer = new byte[512];
            var originalPosition = stream.CanSeek ? stream.Position : 0;

            if (stream.CanSeek)
                stream.Position = 0;

            var bytesRead = await stream.ReadAsync(
                buffer.AsMemory(0, buffer.Length),
                cancellationToken
            );

            if (stream.CanSeek)
                stream.Position = originalPosition;

            if (bytesRead == 0)
                return true;

            for (var i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == 0x00)
                    return false;
            }

            return true;
        }
    }
}
