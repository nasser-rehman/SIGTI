using FluentAssertions;
using SIGTI.Application.Services;
using Xunit;

namespace SIGTI.Application.Tests.Services;

public class FileSecurityValidatorTests
{
    private readonly FileSecurityValidator _validator = new();

    [Theory]
    [InlineData("arquivo.png", true)]
    [InlineData("foto.jpg", true)]
    [InlineData("foto.jpeg", true)]
    [InlineData("documento.pdf", true)]
    [InlineData("pacote.zip", true)]
    [InlineData("log.txt", true)]
    [InlineData("servidor.log", true)]
    [InlineData("virus.exe", false)]
    [InlineData("script.sh", false)]
    [InlineData("script.bat", false)]
    [InlineData("pagina.php", false)]
    [InlineData("", false)]
    public void IsAllowedExtension_Should_Validate_Correctly(
        string fileName,
        bool expected
    )
    {
        var result = _validator.IsAllowedExtension(fileName);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("image/png", true)]
    [InlineData("image/jpeg", true)]
    [InlineData("application/pdf", true)]
    [InlineData("application/zip", true)]
    [InlineData("application/x-zip-compressed", true)]
    [InlineData("text/plain", true)]
    [InlineData("application/x-msdownload", false)]
    [InlineData("application/octet-stream", false)]
    [InlineData("", false)]
    public void IsAllowedContentType_Should_Validate_Correctly(
        string contentType,
        bool expected
    )
    {
        var result = _validator.IsAllowedContentType(contentType);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task ValidateFileSignatureAsync_WhenPngHasValidMagicBytes_ShouldReturnTrue()
    {
        // Magic bytes do PNG: 89 50 4E 47 0D 0A 1A 0A
        var pngBytes = new byte[]
        {
            0x89,
            0x50,
            0x4E,
            0x47,
            0x0D,
            0x0A,
            0x1A,
            0x0A,
            0x00,
            0x01,
        };
        using var stream = new MemoryStream(pngBytes);

        var result = await _validator.ValidateFileSignatureAsync(
            stream,
            "foto.png"
        );

        result.Should().BeTrue();
        stream.Position.Should().Be(0); // Garante que a posição do stream foi restaurada
    }

    [Fact]
    public async Task ValidateFileSignatureAsync_WhenExeDisguisedAsPng_ShouldReturnFalse()
    {
        // Assinatura de executável Windows (MZ: 4D 5A) renomeado como .png
        var exeBytes = new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00 };
        using var stream = new MemoryStream(exeBytes);

        var result = await _validator.ValidateFileSignatureAsync(
            stream,
            "fake.png"
        );

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateFileSignatureAsync_WhenPdfHasValidHeader_ShouldReturnTrue()
    {
        // %PDF
        var pdfBytes = new byte[]
        {
            0x25,
            0x50,
            0x44,
            0x46,
            0x2D,
            0x31,
            0x2E,
            0x37,
        };
        using var stream = new MemoryStream(pdfBytes);

        var result = await _validator.ValidateFileSignatureAsync(
            stream,
            "relatorio.pdf"
        );

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateFileSignatureAsync_WhenTextContainsNullByte_ShouldReturnFalse()
    {
        // Arquivo de texto contendo byte nulo (\0) disfarçado
        var corruptedText = new byte[]
        {
            0x4F,
            0x6C,
            0xC3,
            0xA1,
            0x00,
            0x54,
            0x65,
            0x73,
            0x74,
        };
        using var stream = new MemoryStream(corruptedText);

        var result = await _validator.ValidateFileSignatureAsync(
            stream,
            "notas.txt"
        );

        result.Should().BeFalse();
    }
}
