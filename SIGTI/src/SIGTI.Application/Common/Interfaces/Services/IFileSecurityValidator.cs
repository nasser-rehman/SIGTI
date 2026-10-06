namespace SIGTI.Application.Common.Interfaces.Services
{
    public interface IFileSecurityValidator
    {
        bool IsAllowedExtension(string fileName);
        bool IsAllowedContentType(string contentType);
        Task<bool> ValidateFileSignatureAsync(
            Stream stream,
            string fileName,
            CancellationToken cancellationToken = default
        );
    }
}
