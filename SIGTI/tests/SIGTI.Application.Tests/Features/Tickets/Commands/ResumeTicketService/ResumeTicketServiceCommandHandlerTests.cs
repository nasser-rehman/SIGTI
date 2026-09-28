using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Commands.ResumeTicketService;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.ResumeTicketService
{
    public class ResumeTicketServiceCommandHandlerTests
    {
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ResumeTicketServiceCommandHandler _handler;

        public ResumeTicketServiceCommandHandlerTests()
        {
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ResumeTicketServiceCommandHandler(
                _entityReferenceServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenTicketIsWaitingCustomer_ShouldTransitionToInProgressAndCommit()
        {
            // Arrange
            var ticket = new TicketBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();
            var admin = new UserBuilder().WithRole(Role.Administrator).Build();
            ticket.AssignTechnician(technician, admin, "Atribuição");
            ticket.StartService();
            ticket.WaitCustomer();

            _entityReferenceServiceMock
                .Setup(x =>
                    x.GetRequiredTicketAsync(
                        ticket.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(ticket);

            var command = new ResumeTicketServiceCommand(ticket.Id);

            // Act
            var response = await _handler.Handle(
                command,
                CancellationToken.None
            );

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(ticket.Id);
            response.Status.Should().Be(TicketStatus.InProgress);
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

            var command = new ResumeTicketServiceCommand(nonExistentId);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenTicketIsNotWaitingCustomer_ShouldThrowDomainException()
        {
            // Arrange
            var ticket = new TicketBuilder().Build(); // Status New
            _entityReferenceServiceMock
                .Setup(x =>
                    x.GetRequiredTicketAsync(
                        ticket.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(ticket);

            var command = new ResumeTicketServiceCommand(ticket.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(
                    "O ticket deve estar aguardando o cliente para retomar o atendimento."
                );
        }
    }
}
