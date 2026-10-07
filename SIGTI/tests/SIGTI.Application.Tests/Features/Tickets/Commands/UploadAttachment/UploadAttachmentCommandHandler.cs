using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Commands.UploadAttachment;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.UploadAttachment;

public class UploadAttachmentCommandHandlerTests
{
    private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UploadAttachmentCommandHandler _handler;

    public UploadAttachmentCommandHandlerTests()
    {
        _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UploadAttachmentCommandHandler(
            _entityReferenceServiceMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldUploadToStorage_AddAttachmentToTicket_AndCommit()
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

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredUserAsync(user.Id, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(user);

        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            ticket.Id,
            "evidencia.png",
            "image/png",
            stream.Length,
            stream,
            user.Id
        );

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.FileName.Should().Be("evidencia.png");
        response.ContentType.Should().Be("image/png");
        response.FileSize.Should().Be(stream.Length);
        response.TicketId.Should().Be(ticket.Id);
        response.UploadedById.Should().Be(user.Id);

        ticket
            .Attachments.Should()
            .ContainSingle(a => a.FileName == "evidencia.png");

        _fileStorageServiceMock.Verify(
            x =>
                x.UploadAsync(
                    stream,
                    It.IsAny<string>(),
                    "image/png",
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenTicketIsClosed_ShouldThrowDomainException_AndNotCommit()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var closedTicket = new TicketBuilder()
            .WithCreatedBy(user)
            .BuildAsClosed();

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredTicketAsync(
                    closedTicket.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(closedTicket);

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredUserAsync(user.Id, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(user);

        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            closedTicket.Id,
            "evidencia.png",
            "image/png",
            stream.Length,
            stream,
            user.Id
        );

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Não é possível adicionar anexos a tickets fechados.");

        _fileStorageServiceMock.Verify(
            x =>
                x.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenUnitOfWorkFails_ShouldCleanupUploadedFile_AndRethrow()
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

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredUserAsync(user.Id, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(user);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Erro de persistência no banco de dados"
                )
            );

        using var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadAttachmentCommand(
            ticket.Id,
            "evidencia.png",
            "image/png",
            stream.Length,
            stream,
            user.Id
        );

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro de persistência no banco de dados");

        _fileStorageServiceMock.Verify(
            x =>
                x.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
