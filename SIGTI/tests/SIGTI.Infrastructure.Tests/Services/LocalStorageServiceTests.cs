using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SIGTI.Infrastructure.Storage;
using Xunit;

namespace SIGTI.Infrastructure.Tests.Services;

public class LocalStorageServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly LocalStorageService _storageService;

    public LocalStorageServiceTests()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "sigti_test_storage_" + Guid.NewGuid().ToString("N")
        );
        Directory.CreateDirectory(_testDirectory);

        var options = Options.Create(
            new FileStorageOptions { BasePath = _testDirectory }
        );
        _storageService = new LocalStorageService(options);
    }

    [Fact]
    public async Task UploadAsync_And_DownloadAsync_Should_Persist_And_Retrieve_Content()
    {
        var content = "Conteúdo de teste para upload";
        var bytes = Encoding.UTF8.GetBytes(content);
        using var uploadStream = new MemoryStream(bytes);
        var storageKey = "tickets/test-id/file.txt";

        var resultKey = await _storageService.UploadAsync(
            uploadStream,
            storageKey,
            "text/plain"
        );
        resultKey.Should().Be(storageKey);

        var exists = await _storageService.ExistsAsync(storageKey);
        exists.Should().BeTrue();

        using var downloadStream = await _storageService.DownloadAsync(
            storageKey
        );
        using var reader = new StreamReader(downloadStream, Encoding.UTF8);
        var downloadedContent = await reader.ReadToEndAsync();

        downloadedContent.Should().Be(content);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_File()
    {
        var bytes = Encoding.UTF8.GetBytes("conteúdo");
        using var uploadStream = new MemoryStream(bytes);
        var storageKey = "tickets/delete-test/file.txt";

        await _storageService.UploadAsync(
            uploadStream,
            storageKey,
            "text/plain"
        );
        await _storageService.DeleteAsync(storageKey);

        var exists = await _storageService.ExistsAsync(storageKey);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task UploadAsync_WhenPathTraversalAttempted_ShouldThrowInvalidOperationException()
    {
        using var uploadStream = new MemoryStream(new byte[] { 1, 2, 3 });
        var dangerousKey = "../../windows/system32/cmd.exe";

        var act = () =>
            _storageService.UploadAsync(
                uploadStream,
                dangerousKey,
                "application/octet-stream"
            );

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(
                "Tentativa de acesso a caminho inválido fora do diretório de armazenamento."
            );
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
            catch
            {
                // Limpeza segura de arquivos de teste
            }
        }
    }
}
