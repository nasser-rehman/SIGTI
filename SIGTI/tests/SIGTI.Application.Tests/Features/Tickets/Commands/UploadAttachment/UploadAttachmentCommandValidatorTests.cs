using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Commands.UploadAttachment;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.UploadAttachment;

public class UploadAttachmentCommandValidatorTests
{
    private readonly Mock<IFileSecurityValidator> _fileSecurityValidatorMock;
    private readonly UploadAttachmentCommandValidator _validator;

    public UploadAttachmentCommandValidatorTests()
    {
        _fileSecurityValidatorMock = new Mock<IFileSecurityValidator>();

        _fileSecurityValidatorMock
            .Setup(x => x.IsAllowedExtension(It.IsAny<string>()))
            .Returns(true);

        _fileSecurityValidatorMock
            .Setup(x => x.IsAllowedContentType(It.IsAny<string>()))
            .Returns(true);

        _fileSecurityValidatorMock
            .Setup(x =>
                x.ValidateFileSignatureAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        _validator = new UploadAttachmentCommandValidator(
            _fileSecurityValidatorMock.Object
        );
    }

    [Fact]
    public async Task Validate_WhenValidCommand_ShouldPass()
    {
        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            Guid.NewGuid(),
            "arquivo.png",
            "image/png",
            1024,
            stream,
            Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenFileSizeExceeds10MB_ShouldFail()
    {
        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            Guid.NewGuid(),
            "grande.pdf",
            "application/pdf",
            10 * 1024 * 1024 + 1, // 10 MB + 1 byte
            stream,
            Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FileSize");
    }

    [Fact]
    public async Task Validate_WhenExtensionNotAllowed_ShouldFail()
    {
        _fileSecurityValidatorMock
            .Setup(x => x.IsAllowedExtension("virus.exe"))
            .Returns(false);

        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            Guid.NewGuid(),
            "virus.exe",
            "application/x-msdownload",
            1024,
            stream,
            Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FileName");
    }

    [Fact]
    public async Task Validate_WhenMagicBytesInvalid_ShouldFail()
    {
        _fileSecurityValidatorMock
            .Setup(x =>
                x.ValidateFileSignatureAsync(
                    It.IsAny<Stream>(),
                    "fake.png",
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            Guid.NewGuid(),
            "fake.png",
            "image/png",
            1024,
            stream,
            Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result
            .Errors.Should()
            .Contain(e => e.ErrorMessage.Contains("Magic Bytes"));
    }
}
