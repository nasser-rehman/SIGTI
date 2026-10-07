using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Queries.DownloadAttachment;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.DownloadAttachment;

public class DownloadAttachmentQueryHandlerTests
{
    private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly DownloadAttachmentQueryHandler _handler;

    public DownloadAttachmentQueryHandlerTests()
    {
        _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();

        _handler = new DownloadAttachmentQueryHandler(
            _entityReferenceServiceMock.Object,
            _fileStorageServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnStreamAndMetadata()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var ticket = new TicketBuilder().WithCreatedBy(user).Build();
        var attachment = new AttachmentBuilder()
            .WithTicket(ticket)
            .WithUploadedBy(user)
            .Build();
        ticket.AddAttachment(attachment);

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredTicketAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ticket);

        _fileStorageServiceMock
            .Setup(x =>
                x.ExistsAsync(
                    attachment.StorageKey,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var expectedStream = new MemoryStream([1, 2, 3]);
        _fileStorageServiceMock
            .Setup(x =>
                x.DownloadAsync(
                    attachment.StorageKey,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedStream);

        var query = new DownloadAttachmentQuery(ticket.Id, attachment.Id);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.FileName.Should().Be(attachment.FileName);
        response.ContentType.Should().Be(attachment.ContentType);
        response.FileSize.Should().Be(attachment.FileSize);
        response.Stream.Should().BeSameAs(expectedStream);
    }

    [Fact]
    public async Task Handle_WhenAttachmentNotFoundInTicket_ShouldThrowNotFoundException()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var ticket = new TicketBuilder().WithCreatedBy(user).Build();

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredTicketAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ticket);

        var query = new DownloadAttachmentQuery(ticket.Id, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*não encontrado para este ticket*");
    }

    [Fact]
    public async Task Handle_WhenPhysicalFileNotFoundInStorage_ShouldThrowNotFoundException()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var ticket = new TicketBuilder().WithCreatedBy(user).Build();
        var attachment = new AttachmentBuilder()
            .WithTicket(ticket)
            .WithUploadedBy(user)
            .Build();
        ticket.AddAttachment(attachment);

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredTicketAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ticket);

        _fileStorageServiceMock
            .Setup(x =>
                x.ExistsAsync(
                    attachment.StorageKey,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var query = new DownloadAttachmentQuery(ticket.Id, attachment.Id);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(
                "O arquivo físico não foi encontrado no armazenamento."
            );
    }
}
