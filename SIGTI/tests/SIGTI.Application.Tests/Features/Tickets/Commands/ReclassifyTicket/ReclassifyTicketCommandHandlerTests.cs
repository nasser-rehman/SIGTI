using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.ReclassifyTicket
{
    public class ReclassifyTicketCommandHandlerTests
    {
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReclassifyTicketCommandHandler _handler;

        public ReclassifyTicketCommandHandlerTests()
        {
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ReclassifyTicketCommandHandler(
                _entityReferenceServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenTicketIsActive_ShouldReclassifyAndCommit()
        {
            // Arrange
            var ticket = new TicketBuilder().Build();
            _entityReferenceServiceMock
                .Setup(x =>
                    x.GetRequiredTicketAsync(
                        ticket.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(ticket);

            var command = new ReclassifyTicketCommand(
                ticket.Id,
                TicketPriority.Critical,
                TicketCategory.Network
            );

            // Act
            var response = await _handler.Handle(
                command,
                CancellationToken.None
            );

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(ticket.Id);
            response.Priority.Should().Be(TicketPriority.Critical);
            response.Category.Should().Be(TicketCategory.Network);
            response.UpdatedAt.Should().NotBeNull();

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _entityReferenceServiceMock
                .Setup(x =>
                    x.GetRequiredTicketAsync(
                        nonExistentId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(
                    new NotFoundException(nameof(Ticket), nonExistentId)
                );

            var command = new ReclassifyTicketCommand(
                nonExistentId,
                TicketPriority.High,
                TicketCategory.Hardware
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenTicketIsClosed_ShouldThrowDomainException()
        {
            // Arrange
            var ticket = new TicketBuilder().BuildAsClosed();
            _entityReferenceServiceMock
                .Setup(x =>
                    x.GetRequiredTicketAsync(
                        ticket.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(ticket);

            var command = new ReclassifyTicketCommand(
                ticket.Id,
                TicketPriority.High,
                TicketCategory.Hardware
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(
                    "Não é possível reclassificar tickets resolvidos ou fechados."
                );
        }
    }
}
